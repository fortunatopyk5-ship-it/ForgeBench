using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    public enum ProceduralSurfaceKind
    {
        Graphite,
        DarkMetal,
        BrushedMetal,
        Copper,
        Rubber,
        Pcb,
        Polymer
    }

    /// <summary>
    /// Tiny original tileable texture fallback used until authored PBR maps are available.
    /// Textures are deterministic, generated once, and deliberately subtle so they add material
    /// breakup without pretending to be final scanned assets.
    /// </summary>
    public static class ProceduralSurfaceLibrary
    {
        private const int TextureSize = 64;
        private static readonly Dictionary<ProceduralSurfaceKind, Texture2D> textures = new Dictionary<ProceduralSurfaceKind, Texture2D>();

        public static Texture2D Texture(ProceduralSurfaceKind kind)
        {
            if (textures.TryGetValue(kind, out Texture2D cached) && cached != null) return cached;

            Texture2D texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, true, false)
            {
                name = "ForgeBench_Procedural_" + kind,
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear,
                anisoLevel = 4,
                hideFlags = HideFlags.DontSave
            };

            Color32[] pixels = new Color32[TextureSize * TextureSize];
            for (int y = 0; y < TextureSize; y++)
            for (int x = 0; x < TextureSize; x++)
                pixels[y * TextureSize + x] = Sample(kind, x, y);

            texture.SetPixels32(pixels);
            texture.Apply(true, true);
            textures[kind] = texture;
            return texture;
        }

        public static void ApplyToMaterial(Material material, ProceduralSurfaceKind kind, float tiling = 3f)
        {
            if (material == null) return;
            Texture2D texture = Texture(kind);
            Vector2 scale = Vector2.one * Mathf.Max(.25f, tiling);
            if (material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", texture);
                material.SetTextureScale("_BaseMap", scale);
            }
            else if (material.HasProperty("_MainTex"))
            {
                material.SetTexture("_MainTex", texture);
                material.SetTextureScale("_MainTex", scale);
            }
            material.enableInstancing = true;
        }

        /// <summary>Pure deterministic sample used both by runtime generation and EditMode regression tests.</summary>
        public static Color32 Sample(ProceduralSurfaceKind kind, int x, int y)
        {
            int seed = 17 + (int)kind * 977;
            uint h = Hash(x, y, seed);
            float noise = ((h & 1023u) / 1023f) * 2f - 1f;
            float fine = (((h >> 10) & 255u) / 255f) * 2f - 1f;
            float value;

            switch (kind)
            {
                case ProceduralSurfaceKind.BrushedMetal:
                {
                    uint row = Hash(0, y, seed + 41);
                    float streak = ((row & 1023u) / 1023f) * 2f - 1f;
                    value = .90f + streak * .065f + fine * .018f;
                    if ((y & 15) == 0) value += .025f;
                    break;
                }
                case ProceduralSurfaceKind.Copper:
                {
                    uint row = Hash(0, y, seed + 73);
                    float streak = ((row & 1023u) / 1023f) * 2f - 1f;
                    value = .90f + streak * .055f + fine * .020f;
                    break;
                }
                case ProceduralSurfaceKind.Pcb:
                    value = .84f + noise * .040f;
                    if (x % 19 == 0 || y % 23 == 0 || (x + y) % 31 == 0) value += .085f;
                    break;
                case ProceduralSurfaceKind.Rubber:
                    value = .82f + noise * .035f + fine * .020f;
                    break;
                case ProceduralSurfaceKind.Graphite:
                    value = .86f + noise * .045f + fine * .018f;
                    break;
                case ProceduralSurfaceKind.DarkMetal:
                    value = .88f + noise * .035f + fine * .012f;
                    break;
                default:
                    value = .91f + noise * .025f + fine * .012f;
                    break;
            }

            byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(value * 255f), 0, 255);
            return new Color32(b, b, b, 255);
        }

        private static uint Hash(int x, int y, int seed)
        {
            unchecked
            {
                uint h = (uint)seed;
                h ^= (uint)x * 0x9E3779B9u;
                h = (h << 13) | (h >> 19);
                h ^= (uint)y * 0x85EBCA6Bu;
                h ^= h >> 16;
                h *= 0x7FEB352Du;
                h ^= h >> 15;
                h *= 0x846CA68Bu;
                h ^= h >> 16;
                return h;
            }
        }
    }
}
