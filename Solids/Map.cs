using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Solids
{
    public class Map : Solid
    {
        public List<SolidTile> solidTiles = new List<SolidTile>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="mapWidth">Max number of horizontal tiles</param>
        /// <param name="mapHeight">Max number of vertical tiles</param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="mapOrganisation">0 for nothing placed, a 1 for a tile placed, lenght of the array must be equal to mapWidth * mapHeight</param>
        public Map(Vector2 position, int tileWidth, int tileHeight, int[,] mapOrganisation) : base(position, mapOrganisation.GetLength(0) * tileWidth, mapOrganisation.GetLength(1) * tileHeight)
        {
            for(int y = 0; y < mapOrganisation.GetLength(0); y++)
            {
                for(int x = 0; x < mapOrganisation.GetLength(1); x++)
                {
                    if (mapOrganisation[y, x] == 1)
                        solidTiles.Add(new SolidTile(Drawing.pointTexture, new Vector2(position.X + x * tileWidth, position.Y + y * tileHeight), tileWidth, tileHeight));
                }
            }
        }

        public override void Render()
        {
            foreach (SolidTile s in solidTiles)
                s.Render();
        }
    }
}