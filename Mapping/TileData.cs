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
        public static Dictionary<string, Texture2D> GrassTileSet = TextureManager.LoadAllFilesInFolder<Texture2D>("GrassTileSet");
        public static readonly Dictionary<int, Dictionary<string, Texture2D>> TileSets = new Dictionary<int, Dictionary<string, Texture2D>>()
        {
            { 1, GrassTileSet }
        };
    }
}
