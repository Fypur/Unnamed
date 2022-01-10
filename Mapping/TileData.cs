using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Basic_platformer
{
    public static class TileData
    {
        public static Dictionary<string, Texture2D> GrassTileSet = LoadAllFilesInFolder<Texture2D>("GrassTileSet");
        public static readonly Dictionary<int, Dictionary<string, Texture2D>> TileSets = new Dictionary<int, Dictionary<string, Texture2D>>()
        {
            { 1, GrassTileSet }
        };

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
