using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Platformer
{
    public static class Saving
    {
        public static SaveData LastSaveData = GetLastSaveData();
        public class SaveData
        {
            public int WorldUnlocked { get; set; }
            public string CurrentLevel { get; set; }
            public int CurrentWorld { get; set; }

            public SaveData()
            {
                WorldUnlocked = LastSaveData != null ? LastSaveData.WorldUnlocked : Platformer.WorldsUnlocked;
                CurrentLevel = LastSaveData != null ? LastSaveData.CurrentLevel : Platformer.InitLevel;
                CurrentWorld = LastSaveData != null ? LastSaveData.CurrentWorld : Platformer.InitWorld;
            }
        }

        public static void Save(SaveData save)
        {
            LastSaveData = save;

            string s = JsonSerializer.Serialize<SaveData>(save);
            using StreamWriter f = File.CreateText("Saves/save.json");
            f.Write(s);
        }

        public static bool Load()
        {
            if (File.Exists("Saves/save.json"))
            {
                SaveData s = JsonSerializer.Deserialize<SaveData>(File.ReadAllText("Saves/save.json"));
                Platformer.InitLevel = s.CurrentLevel;
                Platformer.InitWorld = s.CurrentWorld;
                Platformer.WorldsUnlocked = s.WorldUnlocked;
                Platformer.RefreshWorld();

                return true;
            }
            else
                return false;
        }

        private static SaveData GetLastSaveData()
        {
            Load();
            return new SaveData()
            {
                WorldUnlocked = Platformer.WorldsUnlocked,
                CurrentLevel = Platformer.InitLevel,
                CurrentWorld = Platformer.InitWorld,
            };
        }
    }
}
