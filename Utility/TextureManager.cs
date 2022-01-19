using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Basic_platformer
{
    public static class TextureManager
    {
        public static string contentDirName = new DirectoryInfo(Platformer.instance.Content.RootDirectory).FullName;
        public static Dictionary<string, Texture2D> Textures = GetAllTextures();
        public static Texture2D GetTexture(string textureID)
            => Textures[textureID];

        public static void Initialize() { }

        public static Dictionary<string, Texture2D> GetAllTextures()
        {
            ContentManager content = Platformer.instance.Content;
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "\\Graphics");
            
            Dictionary<string, Texture2D> dict = new Dictionary<string, Texture2D>();
            GetAllFiles("", dict);

            return dict;
        }

        private static void GetAllFiles(string folderName, Dictionary<string, Texture2D> d)
        {
            ContentManager content = Platformer.instance.Content;
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "\\Graphics\\" + folderName);

            foreach (FileInfo file in dir.GetFiles())
            {
                string name = file.FullName.Substring(file.FullName.LastIndexOf("Graphics\\") + 9);
                string key = name.Substring(0, name.Length - 4);
                d[key] = content.Load<Texture2D>("Graphics/" + key);
            }
            
            foreach (DirectoryInfo direct in dir.GetDirectories())
                GetAllFiles(direct.FullName.Substring(contentDirName.Length + "Graphics\\".Length), d);
        }

        public static Dictionary<string, T> LoadAllFilesInFolder<T>(string folderName)
        {
            ContentManager content = Platformer.instance.Content;
            DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "\\Graphics\\" + folderName);

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
