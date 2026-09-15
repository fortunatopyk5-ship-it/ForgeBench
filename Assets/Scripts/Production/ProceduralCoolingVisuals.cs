using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Original runtime-authored cooling hardware used when no cleared production prefab exists.
    /// The geometry is intentionally modular and brand-neutral: fan frame, rotor/blades, radiator,
    /// pump/block, tubes and tower cooler are generated from dimensions already present in the catalog.
    /// Gameplay state remains authoritative; this class only creates presentation geometry.
    /// </summary>
    public static class ProceduralCoolingVisuals
    {
        private static Mesh fanBladeMesh;
        private static Mesh torusMesh;

        public static bool IsAio(HardwareDefinition d)
        {
            if (d == null) return false;
            if (HasTag(d, "aio") || HasTag(d, "liquid")) return true;
            string text = ((d.id ?? string.Empty) + " " + (d.model ?? string.Empty)).ToLowerInvariant();
            return text.Contains("aio") || text.Contains("liquid");
        }

        public static int ResolveRadiatorMm(HardwareDefinition d)
        {
            if (d == null) return 240;
            string text = ((d.id ?? string.Empty) + " " + (d.model ?? string.Empty) + " " + string.Join(" ", d.tags ?? new List<string>())).ToLowerInvariant();
            // Explicit authored naming wins over normalized fallback values. This avoids the known
            // catalog-normalization issue where rad240/rad280 entries can otherwise become 360 mm.
            if (text.Contains("280")) return 280;
            if (text.Contains("240")) return 240;
            if (text.Contains("360")) return 360;
            int mm = d.radiatorSupportMm;
            if (mm >= 320) return 360;
            if (mm >= 260) return 280;
            return 240;
        }

        public static int ResolveFanMm(HardwareDefinition d)
        {
            if (d == null) return 120;
            string text = ((d.id ?? string.Empty) + " " + (d.model ?? string.Empty) + " " + string.Join(" ", d.tags ?? new List<string>())).ToLowerInvariant();
            if (text.Contains("140")) return 140;
            if (text.Contains("120")) return 120;
            return d.fanSizeMm >= 135 ? 140 : 120;
        }

        public static bool HasRgb(HardwareDefinition d)
        {
            if (d == null) return false;
            return HasTag(d, "rgb") || HasTag(d, "argb") || ((d.model ?? string.Empty).IndexOf("RGB", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static bool HasTag(HardwareDefinition d, string value)
        {
            return d.tags != null && d.tags.Any(t => string.Equals(t, value, StringComparison.OrdinalIgnoreCase));
        }

        public static GameObject BuildFan(
            Transform parent,
            string name,
            Vector3 localPosition,
            Quaternion localRotation,
            float size,
            Material frameMaterial,
            Material rotorMaterial,
            Material metalMaterial,
            Material accentMaterial,
            bool rgb,
            float spinSpeed,
            bool addCollider)
        {
            size = Mathf.Clamp(size, .14f, .30f);
            float depth = size * .115f;
            float rail = size * .105f;
            float half = size * .5f;

            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = localPosition;
            root.transform.localRotation = localRotation;

            Cube(root.transform, "FrameTop", new Vector3(0, half - rail * .5f, 0), new Vector3(size, rail, depth), frameMaterial);
            Cube(root.transform, "FrameBottom", new Vector3(0, -half + rail * .5f, 0), new Vector3(size, rail, depth), frameMaterial);
            Cube(root.transform, "FrameLeft", new Vector3(-half + rail * .5f, 0, 0), new Vector3(rail, size - rail * 2f, depth), frameMaterial);
            Cube(root.transform, "FrameRight", new Vector3(half - rail * .5f, 0, 0), new Vector3(rail, size - rail * 2f, depth), frameMaterial);

            GameObject shroud = MeshObject(root.transform, "InnerShroud", TorusMesh(), frameMaterial);
            shroud.transform.localScale = new Vector3(size * .92f, size * .92f, depth * .85f);

            GameObject rotor = new GameObject("Rotor");
            rotor.transform.SetParent(root.transform, false);
            rotor.transform.localPosition = new Vector3(0, 0, -depth * .08f);
            SpinVisual spin = rotor.AddComponent<SpinVisual>();
            spin.speed = spinSpeed;

            Cylinder(rotor.transform, "Hub", Vector3.zero, new Vector3(size * .105f, depth * .48f, size * .105f), Quaternion.Euler(90f, 0f, 0f), rotorMaterial);
            Mesh bladeMesh = FanBladeMesh();
            const int bladeCount = 9;
            for (int i = 0; i < bladeCount; i++)
            {
                GameObject blade = MeshObject(rotor.transform, "Blade_" + i, bladeMesh, rotorMaterial);
                blade.transform.localScale = Vector3.one * size;
                blade.transform.localRotation = Quaternion.Euler(0, 0, i * (360f / bladeCount) + 13f);
            }

            float screwOffset = half - rail * .55f;
            for (int x = -1; x <= 1; x += 2)
            for (int y = -1; y <= 1; y += 2)
            {
                Cylinder(root.transform, "CornerBoss", new Vector3(x * screwOffset, y * screwOffset, -depth * .54f),
                    new Vector3(rail * .23f, depth * .12f, rail * .23f), Quaternion.Euler(90f, 0f, 0f), metalMaterial);
            }

            if (rgb && accentMaterial != null)
            {
                GameObject ring = MeshObject(root.transform, "RgbRing", TorusMesh(), accentMaterial);
                ring.transform.localPosition = new Vector3(0, 0, -depth * .58f);
                ring.transform.localScale = new Vector3(size * .79f, size * .79f, depth * .18f);
            }

            if (addCollider)
            {
                BoxCollider collider = root.AddComponent<BoxCollider>();
                collider.size = new Vector3(size, size, depth * 1.25f);
            }
            return root;
        }

        public static GameObject BuildAio(
            Transform parent,
            string name,
            Vector3 pumpPosition,
            Vector3 radiatorPosition,
            Quaternion radiatorRotation,
            int radiatorMm,
            Material frameMaterial,
            Material metalMaterial,
            Material rotorMaterial,
            Material tubeMaterial,
            Material accentMaterial,
            bool rgb,
            bool addCollider)
        {
            radiatorMm = radiatorMm >= 320 ? 360 : radiatorMm >= 260 ? 280 : 240;
            int fanCount = radiatorMm == 360 ? 3 : 2;
            int fanMm = radiatorMm == 280 ? 140 : 120;
            float fanSize = fanMm == 140 ? .245f : .218f;
            float radiatorWidth = fanCount * fanSize + .028f;
            float radiatorHeight = fanSize + .028f;
            float radiatorDepth = .045f;

            GameObject container = new GameObject(name);
            container.transform.SetParent(parent, false);

            GameObject pump = new GameObject("PumpBlock");
            pump.transform.SetParent(container.transform, false);
            pump.transform.localPosition = pumpPosition;
            Cylinder(pump.transform, "PumpHousing", Vector3.zero, new Vector3(.082f, .027f, .082f), Quaternion.Euler(90f, 0f, 0f), frameMaterial);
            Cylinder(pump.transform, "ColdPlate", new Vector3(0, 0, .031f), new Vector3(.070f, .010f, .070f), Quaternion.Euler(90f, 0f, 0f), metalMaterial);
            Cylinder(pump.transform, "PumpCap", new Vector3(0, 0, -.031f), new Vector3(.060f, .009f, .060f), Quaternion.Euler(90f, 0f, 0f), accentMaterial ?? frameMaterial);
            if (addCollider)
            {
                BoxCollider c = pump.AddComponent<BoxCollider>();
                c.size = new Vector3(.18f, .18f, .085f);
            }

            GameObject radiator = new GameObject("Radiator_" + radiatorMm);
            radiator.transform.SetParent(container.transform, false);
            radiator.transform.localPosition = radiatorPosition;
            radiator.transform.localRotation = radiatorRotation;

            float edge = .018f;
            Cube(radiator.transform, "RadTop", new Vector3(0, radiatorHeight * .5f - edge * .5f, 0), new Vector3(radiatorWidth, edge, radiatorDepth), frameMaterial);
            Cube(radiator.transform, "RadBottom", new Vector3(0, -radiatorHeight * .5f + edge * .5f, 0), new Vector3(radiatorWidth, edge, radiatorDepth), frameMaterial);
            Cube(radiator.transform, "RadLeft", new Vector3(-radiatorWidth * .5f + edge * .5f, 0, 0), new Vector3(edge, radiatorHeight, radiatorDepth), frameMaterial);
            Cube(radiator.transform, "RadRight", new Vector3(radiatorWidth * .5f - edge * .5f, 0, 0), new Vector3(edge, radiatorHeight, radiatorDepth), frameMaterial);

            GameObject fins = MeshObject(radiator.transform, "RadiatorFinPack",
                FinPackMesh(radiatorWidth - edge * 2.6f, radiatorHeight - edge * 2.2f, radiatorDepth * .62f, 30), metalMaterial);
            fins.transform.localPosition = new Vector3(0, 0, radiatorDepth * .02f);

            for (int i = 0; i < fanCount; i++)
            {
                float x = (i - (fanCount - 1) * .5f) * fanSize;
                BuildFan(radiator.transform, "RadiatorFan_" + i, new Vector3(x, 0, -radiatorDepth * .72f), Quaternion.identity,
                    fanSize * .94f, frameMaterial, rotorMaterial, metalMaterial, accentMaterial, rgb, 330f + i * 11f, false);
            }

            Vector3 localPortA = new Vector3(radiatorWidth * .40f, radiatorHeight * .36f, radiatorDepth * .25f);
            Vector3 localPortB = new Vector3(radiatorWidth * .40f, radiatorHeight * .18f, radiatorDepth * .25f);
            Vector3 endA = radiatorPosition + radiatorRotation * localPortA;
            Vector3 endB = radiatorPosition + radiatorRotation * localPortB;
            Vector3 startA = pumpPosition + new Vector3(.058f, .035f, -.018f);
            Vector3 startB = pumpPosition + new Vector3(.058f, -.035f, -.018f);
            Vector3 c1a = startA + new Vector3(.14f, .13f, -.08f);
            Vector3 c2a = endA + new Vector3(.04f, -.10f, -.05f);
            Vector3 c1b = startB + new Vector3(.16f, .04f, -.06f);
            Vector3 c2b = endB + new Vector3(.02f, -.06f, -.05f);
            MeshObject(container.transform, "Tube_A", TubeMesh(Bezier(startA, c1a, c2a, endA, 14), .014f, 8), tubeMaterial);
            MeshObject(container.transform, "Tube_B", TubeMesh(Bezier(startB, c1b, c2b, endB, 14), .014f, 8), tubeMaterial);

            return pump;
        }

        public static GameObject BuildAirCooler(
            Transform parent,
            string name,
            Vector3 localPosition,
            Material finMaterial,
            Material frameMaterial,
            Material rotorMaterial,
            Material copperMaterial,
            Material accentMaterial,
            bool rgb,
            bool addCollider)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = localPosition;

            GameObject fins = MeshObject(root.transform, "TowerFinStack", StackedFinMesh(.25f, .285f, .115f, 22), finMaterial);
            fins.transform.localPosition = new Vector3(0, 0, .005f);
            for (int i = 0; i < 4; i++)
            {
                float x = -.075f + i * .05f;
                Cylinder(root.transform, "Heatpipe_" + i, new Vector3(x, -.015f, .018f),
                    new Vector3(.010f, .165f, .010f), Quaternion.identity, copperMaterial);
            }
            Cube(root.transform, "BasePlate", new Vector3(0, -.155f, .018f), new Vector3(.15f, .025f, .10f), copperMaterial);
            BuildFan(root.transform, "TowerFan", new Vector3(0, 0, -.083f), Quaternion.identity, .205f,
                frameMaterial, rotorMaterial, finMaterial, accentMaterial, rgb, 410f, false);

            if (addCollider)
            {
                BoxCollider c = root.AddComponent<BoxCollider>();
                c.size = new Vector3(.27f, .31f, .20f);
            }
            return root;
        }

        private static Mesh FanBladeMesh()
        {
            if (fanBladeMesh != null) return fanBladeMesh;
            fanBladeMesh = new Mesh { name = "ForgeBench_FanBlade" };
            // Normalized, slightly swept wedge. Nine copies form the rotor.
            Vector3[] v =
            {
                new Vector3(.10f,-.032f,-.020f), new Vector3(.42f,-.105f,-.012f), new Vector3(.43f,.025f,-.006f), new Vector3(.13f,.055f,-.016f),
                new Vector3(.10f,-.032f,.020f),  new Vector3(.42f,-.105f,.012f),  new Vector3(.43f,.025f,.006f),  new Vector3(.13f,.055f,.016f)
            };
            int[] t =
            {
                0,2,1, 0,3,2, 4,5,6, 4,6,7,
                0,1,5, 0,5,4, 1,2,6, 1,6,5,
                2,3,7, 2,7,6, 3,0,4, 3,4,7
            };
            fanBladeMesh.vertices = v;
            fanBladeMesh.triangles = t;
            fanBladeMesh.RecalculateNormals();
            fanBladeMesh.RecalculateBounds();
            return fanBladeMesh;
        }

        private static Mesh TorusMesh()
        {
            if (torusMesh != null) return torusMesh;
            const int segments = 36;
            const int sides = 6;
            const float major = .44f;
            const float minor = .045f;
            List<Vector3> vertices = new List<Vector3>((segments + 1) * (sides + 1));
            List<int> triangles = new List<int>(segments * sides * 6);
            for (int i = 0; i <= segments; i++)
            {
                float a = i * Mathf.PI * 2f / segments;
                Vector3 radial = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
                for (int j = 0; j <= sides; j++)
                {
                    float b = j * Mathf.PI * 2f / sides;
                    vertices.Add(radial * (major + Mathf.Cos(b) * minor) + Vector3.forward * (Mathf.Sin(b) * minor));
                }
            }
            for (int i = 0; i < segments; i++)
            for (int j = 0; j < sides; j++)
            {
                int a = i * (sides + 1) + j;
                int b = a + sides + 1;
                triangles.Add(a); triangles.Add(b); triangles.Add(a + 1);
                triangles.Add(a + 1); triangles.Add(b); triangles.Add(b + 1);
            }
            torusMesh = new Mesh { name = "ForgeBench_FanShroud" };
            torusMesh.SetVertices(vertices);
            torusMesh.SetTriangles(triangles, 0);
            torusMesh.RecalculateNormals();
            torusMesh.RecalculateBounds();
            return torusMesh;
        }

        private static Mesh FinPackMesh(float width, float height, float depth, int count)
        {
            List<Vector3> v = new List<Vector3>(count * 8);
            List<int> t = new List<int>(count * 36);
            float usable = width * .96f;
            for (int i = 0; i < count; i++)
            {
                float x = -usable * .5f + usable * (i + .5f) / count;
                AppendBox(v, t, new Vector3(x, 0, 0), new Vector3(Mathf.Max(.003f, usable / count * .34f), height, depth));
            }
            Mesh m = new Mesh { name = "ForgeBench_RadiatorFins" };
            m.SetVertices(v); m.SetTriangles(t, 0); m.RecalculateNormals(); m.RecalculateBounds();
            return m;
        }

        private static Mesh StackedFinMesh(float width, float height, float depth, int count)
        {
            List<Vector3> v = new List<Vector3>(count * 8);
            List<int> t = new List<int>(count * 36);
            for (int i = 0; i < count; i++)
            {
                float y = -height * .5f + height * (i + .5f) / count;
                AppendBox(v, t, new Vector3(0, y, 0), new Vector3(width, Mathf.Max(.0025f, height / count * .28f), depth));
            }
            Mesh m = new Mesh { name = "ForgeBench_TowerFins" };
            m.SetVertices(v); m.SetTriangles(t, 0); m.RecalculateNormals(); m.RecalculateBounds();
            return m;
        }

        private static void AppendBox(List<Vector3> v, List<int> t, Vector3 center, Vector3 size)
        {
            Vector3 h = size * .5f;
            int o = v.Count;
            v.Add(center + new Vector3(-h.x,-h.y,-h.z)); v.Add(center + new Vector3(h.x,-h.y,-h.z));
            v.Add(center + new Vector3(h.x,h.y,-h.z));   v.Add(center + new Vector3(-h.x,h.y,-h.z));
            v.Add(center + new Vector3(-h.x,-h.y,h.z)); v.Add(center + new Vector3(h.x,-h.y,h.z));
            v.Add(center + new Vector3(h.x,h.y,h.z));   v.Add(center + new Vector3(-h.x,h.y,h.z));
            int[] faces = {0,2,1,0,3,2,4,5,6,4,6,7,0,1,5,0,5,4,1,2,6,1,6,5,2,3,7,2,7,6,3,0,4,3,4,7};
            for (int i = 0; i < faces.Length; i++) t.Add(o + faces[i]);
        }

        private static List<Vector3> Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segments)
        {
            List<Vector3> points = new List<Vector3>(segments + 1);
            for (int i = 0; i <= segments; i++)
            {
                float u = i / (float)segments;
                float q = 1f - u;
                points.Add(q*q*q*p0 + 3f*q*q*u*p1 + 3f*q*u*u*p2 + u*u*u*p3);
            }
            return points;
        }

        private static Mesh TubeMesh(List<Vector3> points, float radius, int sides)
        {
            List<Vector3> v = new List<Vector3>(points.Count * sides);
            List<int> t = new List<int>((points.Count - 1) * sides * 6);
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 tangent;
                if (i == 0) tangent = (points[1] - points[0]).normalized;
                else if (i == points.Count - 1) tangent = (points[i] - points[i-1]).normalized;
                else tangent = (points[i+1] - points[i-1]).normalized;
                Vector3 n = Vector3.Cross(tangent, Mathf.Abs(Vector3.Dot(tangent, Vector3.up)) > .92f ? Vector3.right : Vector3.up).normalized;
                Vector3 b = Vector3.Cross(tangent, n).normalized;
                for (int s = 0; s < sides; s++)
                {
                    float a = Mathf.PI * 2f * s / sides;
                    v.Add(points[i] + (n * Mathf.Cos(a) + b * Mathf.Sin(a)) * radius);
                }
            }
            for (int i = 0; i < points.Count - 1; i++)
            for (int s = 0; s < sides; s++)
            {
                int next = (s + 1) % sides;
                int a = i * sides + s;
                int b = i * sides + next;
                int c = (i + 1) * sides + s;
                int d = (i + 1) * sides + next;
                t.Add(a); t.Add(c); t.Add(b);
                t.Add(b); t.Add(c); t.Add(d);
            }
            Mesh m = new Mesh { name = "ForgeBench_AioTube" };
            m.SetVertices(v); m.SetTriangles(t, 0); m.RecalculateNormals(); m.RecalculateBounds();
            return m;
        }

        private static GameObject MeshObject(Transform parent, string name, Mesh mesh, Material material)
        {
            GameObject g = new GameObject(name);
            g.transform.SetParent(parent, false);
            MeshFilter mf = g.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.sharedMaterial = material;
            return g;
        }

        private static GameObject Cube(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = name;
            g.transform.SetParent(parent, false);
            g.transform.localPosition = position;
            g.transform.localScale = scale;
            Renderer r = g.GetComponent<Renderer>(); if (r != null) r.sharedMaterial = material;
            Collider c = g.GetComponent<Collider>(); if (c != null) UnityEngine.Object.Destroy(c);
            return g;
        }

        private static GameObject Cylinder(Transform parent, string name, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            g.name = name;
            g.transform.SetParent(parent, false);
            g.transform.localPosition = position;
            g.transform.localScale = scale;
            g.transform.localRotation = rotation;
            Renderer r = g.GetComponent<Renderer>(); if (r != null) r.sharedMaterial = material;
            Collider c = g.GetComponent<Collider>(); if (c != null) UnityEngine.Object.Destroy(c);
            return g;
        }
    }
}
