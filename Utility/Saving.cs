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
        public int WorldUnlocked { get; set; }
        public string CurrentLevel { get; set; }
        public int CurrentWorld { get; set; }
        public bool CanJetpack { get; set; }

        public SaveData()
        { }
    }

    public static class Saving
    {
        public static SaveData LastSaveData = Load();

        public static void Save(SaveData save)
        {
            LastSaveData = save;

            string s = JsonSerializer.Serialize<SaveData>(save);
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
            Platformer.LoadSave(s);
        }
    }
}
