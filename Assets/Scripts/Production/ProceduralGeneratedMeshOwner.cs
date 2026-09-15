using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Owns only the unique runtime meshes created for fin packs and AIO tubing. Shared cached fan
    /// blade/shroud meshes and Unity primitive meshes are deliberately excluded. Call Release before
    /// replacing the generated hierarchy; OnDestroy is a defensive fallback.
    /// </summary>
    public sealed class ProceduralGeneratedMeshOwner : MonoBehaviour
    {
        private bool released;

        public void Release()
        {
            if (released) return;
            released = true;
            HashSet<Mesh> destroyed = new HashSet<Mesh>();
            MeshFilter[] filters = GetComponentsInChildren<MeshFilter>(true);
            foreach (MeshFilter filter in filters)
            {
                Mesh mesh = filter != null ? filter.sharedMesh : null;
                if (!Owns(mesh) || destroyed.Contains(mesh)) continue;
                destroyed.Add(mesh);
                filter.sharedMesh = null;
                Destroy(mesh);
            }
        }

        public static bool Owns(Mesh mesh)
        {
            if (mesh == null) return false;
            string n = mesh.name ?? string.Empty;
            return n == "ForgeBench_RadiatorFins" || n == "ForgeBench_TowerFins" || n == "ForgeBench_AioTube";
        }

        private void OnDestroy() { Release(); }
    }
}
