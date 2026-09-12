#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ForgeBench.EditorTools
{
    public static class ForgeBenchContentValidator
    {
        [MenuItem("ForgeBench/Validate Content")]
        public static void ValidateMenu()
        {
            ValidateOrThrow();
            Debug.Log("ForgeBench content validation passed.");
        }

        public static void ValidateOrThrow()
        {
            HardwareCatalog catalog = new HardwareCatalog();
            catalog.Load();
            if (catalog.All.Count < 70) throw new InvalidDataException("Hardware catalog must contain at least 70 definitions.");
            if (catalog.All.Select(p => p.id).Distinct(StringComparer.OrdinalIgnoreCase).Count() != catalog.All.Count)
                throw new InvalidDataException("Hardware catalog contains duplicate IDs.");

            foreach (PartCategory category in Enum.GetValues(typeof(PartCategory)))
                if (!catalog.All.Any(p => p.category == category)) throw new InvalidDataException("Missing hardware category: " + category);

            ValidateLocalization("en");
            ValidateLocalization("uk");

            string[] critical =
            {
                "Assets/Scenes/Workshop.unity", "Assets/Resources/Data/hardware.json",
                "Assets/Scripts/Runtime/GameRuntime.cs", "Assets/Scripts/Runtime/SaveService.cs",
                "Assets/Scripts/Simulation/GameSimulation.cs", "Assets/Scripts/Simulation/AssemblyCustomization.cs",
                "Assets/Scripts/UI/GameUI.cs", "Assets/Scripts/World/WorkshopWorld.cs"
            };
            foreach (string path in critical)
                if (!File.Exists(path)) throw new FileNotFoundException("Critical source asset missing.", path);
        }

        private static void ValidateLocalization(string lang)
        {
            TextAsset asset = Resources.Load<TextAsset>("Localization/" + lang);
            if (asset == null) throw new InvalidDataException("Localization table missing: " + lang);
            LocalizationTable table = JsonUtility.FromJson<LocalizationTable>(asset.text);
            if (table?.entries == null || table.entries.Count < 20) throw new InvalidDataException("Localization table too small/invalid: " + lang);
            if (table.entries.Any(e => string.IsNullOrWhiteSpace(e.key))) throw new InvalidDataException("Localization contains blank key: " + lang);
            if (table.entries.Select(e => e.key).Distinct().Count() != table.entries.Count) throw new InvalidDataException("Localization duplicate key: " + lang);
        }
    }
}
#endif
