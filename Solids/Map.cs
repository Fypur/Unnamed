using Basic_platformer.Utility;
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
        public MapData Data = new MapData();

        /// <summary>
        /// Map constructor
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
                        Data.Solids.Add(new SolidTile(Drawing.pointTexture, new Vector2(position.X + x * tileWidth, position.Y + y * tileHeight), tileWidth, tileHeight));
                        Data.solidTiles.Add(new SolidTile(Drawing.pointTexture, new Vector2(position.X + x * tileWidth, position.Y + y * tileHeight), tileWidth, tileHeight));
                    }
                        
                }
            }

            GrapplingPoint gP = new GrapplingPoint(new Vector2(Platformer.ScreenSize.X / 2, 60));
            Data.Solids.Add(gP);
            Data.GrapplingSolids.Add(gP);
            PulledPlatform pulled = new PulledPlatform(new Vector2(30 ,Platformer.ScreenSize.Y - 400), 200, 10, new Vector2(30 + 200, Platformer.ScreenSize.Y - 400), 2f, Ease.QuintOut, true);
            Data.Solids.Add(pulled);
            GrapplingTrigger platformTrigger = new GrapplingTrigger(new Vector2(pulled.Pos.X + pulled.Width, pulled.Pos.Y), true, pulled.movingTime, pulled.Pulled);
            Data.GrapplingSolids.Add(platformTrigger);
            Data.Triggers.Add(platformTrigger);
        }

        public override void Update()
        {
            for (int i = Data.Solids.Count - 1; i >= 0; i--)
                Data.Solids[i].Update();

            for (int i = Data.Triggers.Count - 1; i >= 0; i--)
                Data.Triggers[i].Update();
        }

        public override void Render()
        {
            foreach (Solid s in Data.Solids)
                    s.Render();
        }
    }
}