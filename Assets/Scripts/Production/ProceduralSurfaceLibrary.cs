using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    public enum ProceduralSurfaceProfile
    {
        None,
        GraphitePowderCoat,
        BrushedAluminum,
        EsdTeal,
        PcbGreen,
        Copper,
        BlackPlastic,
        Rubber,
        WarmWhitePaint,
        Concrete,
        Label
    }

    /// <summary>
    /// Small runtime PBR fallback library. It deliberately generates neutral, brand-free textures
    /// so the workshop does not depend on missing external art. Authored materials can replace it later.
    /// </summary>
    public sealed class ProceduralSurfaceLibrary
    {
        private readonly Shader shader;
        private readonly int textureSize;
        private readonly Dictionary<ProceduralSurfaceProfile, Material> materials = new Dictionary<ProceduralSurfaceProfile, Material>();
        private readonly List<Texture2D> textures = new List<Texture2D>();

        public ProceduralSurfaceLibrary(Shader targetShader, int size)
        {
            shader = targetShader;
            textureSize = Mathf.Clamp(size, 32, 256);
        }

        public Material Get(ProceduralSurfaceProfile profile)
        {
            if (profile == ProceduralSurfaceProfile.None) return null;
            Material existing;
            if (materials.TryGetValue(profile, out existing) && existing != null) return existing;
            Material created = BuildMaterial(profile);
            materials[profile] = created;
            return created;
        }

        public void Dispose()
        {
            foreach (Material material in materials.Values) if (material != null) UnityEngine.Object.Destroy(material);
            materials.Clear();
            foreach (Texture2D texture in textures) if (texture != null) UnityEngine.Object.Destroy(texture);
            textures.Clear();
        }

        public static ProceduralSurfaceProfile Classify(string semanticName)
        {
            string n = (semanticName ?? string.Empty).ToLowerInvariant();
            if (n.Length == 0) return ProceduralSurfaceProfile.None;

            if (n.Contains("floor")) return ProceduralSurfaceProfile.Concrete;
            if (n.Contains("backwall") || n.Contains("leftwall") || n.Contains("rightwall")) return ProceduralSurfaceProfile.WarmWhitePaint;
            if (n.Contains("main assembly bench/top") || n.Contains("diagnostic bench/top") || n.Contains("board repair station/top")) return ProceduralSurfaceProfile.EsdTeal;
            if (n.Contains("packaging station/top")) return ProceduralSurfaceProfile.GraphitePowderCoat;
            if (n.Contains("warehouse") || n.Contains("shelf") || n.Contains("/leg") || n.Contains("/post")) return ProceduralSurfaceProfile.BrushedAluminum;

            if (n.Contains("motherboard") || n.Contains("nvme_") || n.Contains("pcb")) return ProceduralSurfaceProfile.PcbGreen;
            if (n.Contains("ramcontacts") || n.Contains("trace_") || n.Contains("heatpipe") || n.Contains("coldplate") || n.Contains("copper")) return ProceduralSurfaceProfile.Copper;
            if (n.Contains("backplate") || n.Contains("shield") || n.Contains("grille") || n.Contains("screw") || n.Contains("fastener") || n.Contains("cornerboss")) return ProceduralSurfaceProfile.BrushedAluminum;
            if (n.Contains("ramchip") || n.Contains("chipset") || n.Contains("vrm_") || n.Contains("controller") || n.Contains("fanblade") || n.Contains("fanrotor") || n.Contains("rotor") || n.Contains("hub")) return ProceduralSurfaceProfile.BlackPlastic;
            if (n.Contains("cable") || n.Contains("tube") || n.Contains("rubber")) return ProceduralSurfaceProfile.Rubber;
            if (n.Contains("label")) return ProceduralSurfaceProfile.Label;
            if (n.Contains("case") || n.Contains("psu") || n.Contains("gpu") || n.Contains("radiator") || n.Contains("fanframe") || n.Contains("frame") || n.Contains("bezel") || n.Contains("panelmount")) return ProceduralSurfaceProfile.GraphitePowderCoat;
            return ProceduralSurfaceProfile.None;
        }

        public static float Noise01(int x, int y, int seed)
        {
            unchecked
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + seed * 1442695041);
                h = (h ^ (h >> 13)) * 1274126177u;
                h ^= h >> 16;
                return (h & 0x00FFFFFFu) / 16777215f;
            }
        }

        private Material BuildMaterial(ProceduralSurfaceProfile profile)
        {
            Shader useShader = shader != null ? shader : Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material material = new Material(useShader) { name = "FB_Procedural_" + profile };
            Color baseColor;
            float metallic;
            float smoothness;
            float bump;
            ResolveParameters(profile, out baseColor, out metallic, out smoothness, out bump);

            Texture2D albedo = BuildAlbedo(profile, baseColor);
            Texture2D normal = BuildNormal(profile, bump);
            textures.Add(albedo);
            textures.Add(normal);

            if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", albedo);
            if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", albedo);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", Color.white);
            if (material.HasProperty("_Color")) material.SetColor("_Color", Color.white);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", smoothness);
            if (material.HasProperty("_BumpMap"))
            {
                material.SetTexture("_BumpMap", normal);
                if (material.HasProperty("_BumpScale")) material.SetFloat("_BumpScale", 1f);
                material.EnableKeyword("_NORMALMAP");
            }
            return material;
        }

        private Texture2D BuildAlbedo(ProceduralSurfaceProfile profile, Color baseColor)
        {
            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, true, false)
            {
                name = "FB_" + profile + "_Albedo",
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear,
                anisoLevel = 2
            };
            Color32[] pixels = new Color32[textureSize * textureSize];
            int seed = 41 + (int)profile * 101;
            for (int y = 0; y < textureSize; y++)
            for (int x = 0; x < textureSize; x++)
            {
                float noise = Noise01(x, y, seed) - .5f;
                float pattern = Pattern(profile, x, y, textureSize);
                float value = 1f + noise * .065f + pattern;
                Color c = new Color(
                    Mathf.Clamp01(baseColor.r * value),
                    Mathf.Clamp01(baseColor.g * value),
                    Mathf.Clamp01(baseColor.b * value), 1f);
                pixels[y * textureSize + x] = c;
            }
            texture.SetPixels32(pixels);
            texture.Apply(true, true);
            return texture;
        }

        private Texture2D BuildNormal(ProceduralSurfaceProfile profile, float strength)
        {
            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false, true)
            {
                name = "FB_" + profile + "_Normal",
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear
            };
            Color32[] pixels = new Color32[textureSize * textureSize];
            int seed = 73 + (int)profile * 131;
            for (int y = 0; y < textureSize; y++)
            for (int x = 0; x < textureSize; x++)
            {
                float left = Height(profile, x - 1, y, seed);
                float right = Height(profile, x + 1, y, seed);
                float down = Height(profile, x, y - 1, seed);
                float up = Height(profile, x, y + 1, seed);
                Vector3 n = new Vector3((left - right) * strength, (down - up) * strength, 1f).normalized;
                pixels[y * textureSize + x] = new Color(n.x * .5f + .5f, n.y * .5f + .5f, n.z * .5f + .5f, 1f);
            }
            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return texture;
        }

        private float Height(ProceduralSurfaceProfile profile, int x, int y, int seed)
        {
            int xx = (x % textureSize + textureSize) % textureSize;
            int yy = (y % textureSize + textureSize) % textureSize;
            return Noise01(xx, yy, seed) * .60f + Pattern(profile, xx, yy, textureSize) * 5f;
        }

        private static float Pattern(ProceduralSurfaceProfile profile, int x, int y, int size)
        {
            float fx = x / (float)Mathf.Max(1, size - 1);
            float fy = y / (float)Mathf.Max(1, size - 1);
            switch (profile)
            {
                case ProceduralSurfaceProfile.BrushedAluminum:
                case ProceduralSurfaceProfile.Copper:
                    return Mathf.Sin(fy * Mathf.PI * 96f) * .018f + Mathf.Sin(fy * Mathf.PI * 31f) * .010f;
                case ProceduralSurfaceProfile.PcbGreen:
                    return ((x % 24 == 0 || y % 28 == 0) ? .025f : 0f) + Mathf.Sin((fx + fy) * Mathf.PI * 8f) * .006f;
                case ProceduralSurfaceProfile.Concrete:
                    return Mathf.Sin(fx * Mathf.PI * 7f) * Mathf.Sin(fy * Mathf.PI * 5f) * .025f;
                case ProceduralSurfaceProfile.EsdTeal:
                    return Mathf.Sin((fx * 5f + fy * 7f) * Mathf.PI) * .008f;
                default:
                    return 0f;
            }
        }

        private static void ResolveParameters(ProceduralSurfaceProfile profile, out Color color, out float metallic, out float smoothness, out float bump)
        {
            color = new Color(.18f, .19f, .20f); metallic = .05f; smoothness = .35f; bump = .45f;
            switch (profile)
            {
                case ProceduralSurfaceProfile.GraphitePowderCoat: color = new Color(.050f, .058f, .065f); metallic = .58f; smoothness = .34f; bump = .45f; break;
                case ProceduralSurfaceProfile.BrushedAluminum: color = new Color(.53f, .56f, .59f); metallic = .92f; smoothness = .62f; bump = .72f; break;
                case ProceduralSurfaceProfile.EsdTeal: color = new Color(.035f, .25f, .27f); metallic = .08f; smoothness = .26f; bump = .38f; break;
                case ProceduralSurfaceProfile.PcbGreen: color = new Color(.035f, .18f, .105f); metallic = .18f; smoothness = .42f; bump = .28f; break;
                case ProceduralSurfaceProfile.Copper: color = new Color(.68f, .30f, .095f); metallic = .90f; smoothness = .55f; bump = .62f; break;
                case ProceduralSurfaceProfile.BlackPlastic: color = new Color(.022f, .026f, .031f); metallic = .03f; smoothness = .30f; bump = .30f; break;
                case ProceduralSurfaceProfile.Rubber: color = new Color(.014f, .017f, .020f); metallic = 0f; smoothness = .14f; bump = .50f; break;
                case ProceduralSurfaceProfile.WarmWhitePaint: color = new Color(.80f, .79f, .75f); metallic = .02f; smoothness = .30f; bump = .22f; break;
                case ProceduralSurfaceProfile.Concrete: color = new Color(.25f, .27f, .28f); metallic = 0f; smoothness = .18f; bump = .70f; break;
                case ProceduralSurfaceProfile.Label: color = new Color(.72f, .72f, .68f); metallic = .02f; smoothness = .38f; bump = .12f; break;
            }
        }
    }
}
