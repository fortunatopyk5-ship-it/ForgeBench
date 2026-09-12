using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public sealed class HardwareCatalog
    {
        private readonly Dictionary<string, HardwareDefinition> byId = new Dictionary<string, HardwareDefinition>(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyCollection<HardwareDefinition> All => byId.Values;

        public void Load()
        {
            byId.Clear();
            TextAsset asset = Resources.Load<TextAsset>("Data/hardware");
            if (asset == null) throw new InvalidOperationException("Missing Resources/Data/hardware.json");
            HardwareCatalogData data = JsonUtility.FromJson<HardwareCatalogData>(asset.text);
            if (data == null || data.parts == null || data.parts.Count == 0) throw new InvalidOperationException("Hardware catalog is empty or invalid.");
            foreach (HardwareDefinition part in data.parts)
            {
                if (part == null || string.IsNullOrWhiteSpace(part.id)) continue;
                byId[part.id] = part;
            }
        }

        public HardwareDefinition Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            HardwareDefinition result;
            return byId.TryGetValue(id, out result) ? result : null;
        }

        public List<HardwareDefinition> ByCategory(PartCategory category)
        {
            return byId.Values.Where(p => p.category == category).OrderBy(p => p.price).ToList();
        }

        public List<HardwareDefinition> Search(string text, PartCategory? category = null)
        {
            text = (text ?? string.Empty).Trim().ToLowerInvariant();
            return byId.Values.Where(p => (!category.HasValue || p.category == category) &&
                (text.Length == 0 || (p.brand + " " + p.model + " " + p.id).ToLowerInvariant().Contains(text)))
                .OrderBy(p => p.price).ToList();
        }
    }
}
