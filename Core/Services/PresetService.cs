using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BoostX.Models;

namespace BoostX.Core.Services
{
    public class UserPreset
    {
        public string PresetName { get; set; } = "Custom Preset";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Dictionary<string, bool> TweaksState { get; set; } = new();
    }

    public static class PresetService
    {
        public static string ExportPreset(IEnumerable<TweakItem> allTweaks, string presetName = "MyPreset")
        {
            var preset = new UserPreset { PresetName = presetName };
            foreach (var tweak in allTweaks)
            {
                preset.TweaksState[tweak.Id] = tweak.IsChecked;
            }
            return JsonSerializer.Serialize(preset, new JsonSerializerOptions { WriteIndented = true });
        }

        public static void SaveToFile(string filePath, IEnumerable<TweakItem> allTweaks, string presetName = "MyPreset")
        {
            var json = ExportPreset(allTweaks, presetName);
            File.WriteAllText(filePath, json);
        }

        public static UserPreset? LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<UserPreset>(json);
        }

        public static void ApplyPreset(UserPreset preset, IEnumerable<TweakItem> allTweaks)
        {
            foreach (var tweak in allTweaks)
            {
                if (preset.TweaksState.TryGetValue(tweak.Id, out bool shouldBeChecked))
                {
                    if (tweak.IsChecked != shouldBeChecked)
                    {
                        tweak.IsChecked = shouldBeChecked;
                    }
                }
            }
        }
    }
}