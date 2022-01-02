using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Basic_platformer.Mapping
{
    public static class TileData
    {
        public static readonly Dictionary<string, Texture2D> GrassTileSet;

        static TileData()
        {
            GrassTileSet = LoadAllFilesInFolder<Texture2D>("GrassTileSet");
        }

        private static Dictionary<string, T> LoadAllFilesInFolder<T>(string folderName)
        {
            ContentManager content = Platformer.instance.Content;
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "\\" + folderName);

            Dictionary<string, T> d = new Dictionary<string, T>();

            foreach (FileInfo file in dir.GetFiles())
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                d[key] = content.Load<T>(dir.FullName + '/' + key);
            }

            return d;
        }
    }
}
