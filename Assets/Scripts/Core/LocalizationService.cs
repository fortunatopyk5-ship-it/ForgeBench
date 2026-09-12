using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    [Serializable] public class LocalizationEntry { public string key; public string value; }
    [Serializable] public class LocalizationTable { public List<LocalizationEntry> entries = new List<LocalizationEntry>(); }

    public sealed class LocalizationService
    {
        private readonly Dictionary<string,string> table = new Dictionary<string,string>();
        private string loadedLanguage;
        public void Load(string language)
        {
            if (loadedLanguage == language && table.Count > 0) return;
            table.Clear(); loadedLanguage = language;
            TextAsset asset = Resources.Load<TextAsset>("Localization/" + language);
            if (asset == null) asset = Resources.Load<TextAsset>("Localization/en");
            if (asset == null) return;
            LocalizationTable data = JsonUtility.FromJson<LocalizationTable>(asset.text);
            if (data?.entries == null) return;
            foreach (LocalizationEntry e in data.entries) if (!string.IsNullOrEmpty(e.key)) table[e.key] = e.value;
        }
        public string T(string key, string fallback)
        {
            string v; return table.TryGetValue(key, out v) ? v : fallback;
        }
    }
}
