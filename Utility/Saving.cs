using Fiourp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Platformer
{
    public class SaveData
    {
        public int? WorldUnlocked { get; set; }
        public string CurrentLevel { get; set; }
        public int? CurrentWorld { get; set; }
        public bool? CanJetpack { get; set; }
        public int? ScreenSize { get; set; }
        public int? MasterVolume { get; set; }
        public int? MusicVolume { get; set; }
        public int? SFXVolume { get; set; }
        public Control[] jumpControls { get; set; }
        public Control[] jetpackControls { get; set; }
        public Control[] swingControls { get; set; }
        public Control[] upControls { get; set; }
        public Control[] downControls { get; set; }
        public Control[] leftControls { get; set; }
        public Control[] rightControls { get; set; }

        public SaveData()
        {

        }
    }

    public static class Saving
    {
        public static SaveData LastSaveData = Load();
        public static Type SaveType = typeof(SaveData);

        public static void Save(SaveData save)
        {
            //LastSaveData = save;

            SaveData l = Load();

            if (l != null && save != null)
            {
                foreach (System.Reflection.PropertyInfo property in SaveType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    object toValue = SaveType.GetProperty(property.Name).GetValue(save, null);
                    if (toValue == null)
                    {
                        object selfValue = SaveType.GetProperty(property.Name).GetValue(l, null);

                        if(selfValue != null)
                        {
                            //var val = Convert.ChangeType(selfValue, property.PropertyType);
                            property.SetValue(save, selfValue, null);
                        }
                    }
                }
            }

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string s = JsonSerializer.Serialize<SaveData>(save, options);
            using StreamWriter f = File.CreateText("Saves/save.json");
            f.Write(s);
        }

        public static SaveData Load()
        {
            if (File.Exists("Saves/save.json"))
                return JsonSerializer.Deserialize<SaveData>(File.ReadAllText("Saves/save.json"));
            
            return null;
        }
        
        public static void SaveAndLoad(SaveData s)
        {
            Save(s);
            Platformer.LoadWorldSave(s);
        }
    }
}
