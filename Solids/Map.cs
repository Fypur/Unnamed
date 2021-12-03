using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Basic_platformer.Solids
{
    public class Map : Solid
    {
        public int TileWidth;
        public int TileHeight;
        public int[,] MapOrganisation;
        public List<SolidTile> solidTiles = new List<SolidTile>();
        public MapData data = new MapData();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="mapWidth">Max number of horizontal tiles</param>
        /// <param name="mapHeight">Max number of vertical tiles</param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="mapOrganisation">0 for nothing placed, a 1 for a tile placed, lenght of the array must be equal to mapWidth * mapHeight</param>
        public Map(Vector2 position, int tileWidth, int tileHeight, int[,] mapOrganisation) : base(position, mapOrganisation.GetLength(1) * tileWidth, mapOrganisation.GetLength(0) * tileHeight)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            MapOrganisation = mapOrganisation;

            for(int y = 0; y < mapOrganisation.GetLength(0); y++)
            {
                for(int x = 0; x < mapOrganisation.GetLength(1); x++)
                {
                    if (mapOrganisation[y, x] == 1)
                    {
                        data.solids.Add(new SolidTile(Drawing.pointTexture, new Vector2(position.X + x * tileWidth, position.Y + y * tileHeight), tileWidth, tileHeight));
                        data.solidTiles.Add(new SolidTile(Drawing.pointTexture, new Vector2(position.X + x * tileWidth, position.Y + y * tileHeight), tileWidth, tileHeight));
                    }
                        
                }
            }

            GrapplingPoint gP = new GrapplingPoint(new Vector2(Platformer.ScreenSize.X / 2, 60));
            data.solids.Add(gP);
            data.grapplingPoints.Add(gP);
            GrapplingPoint gP2 = new GrapplingPoint(new Vector2(Platformer.ScreenSize.X / 2 + 500, 60));
            data.solids.Add(gP2);
            data.grapplingPoints.Add(gP2);
        }

        public override void Render()
        {
            foreach (Solid s in data.solids)
                    s.Render();
        }
    }
}