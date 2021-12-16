using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Basic_platformer.Solids;
using Basic_platformer.Entities;


namespace Basic_platformer.Mapping
{
    public class Map
    {
        public int Width;
        public int Height;
        public int TileWidth;
        public int TileHeight;
        public int[,] MapOrganisation;
        public List<SolidTile> solidTiles = new List<SolidTile>();
        public MapData Data;
        public Level currentLevel;

        /// <summary>
        /// Map constructor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="mapWidth">Max number of horizontal tiles</param>
        /// <param name="mapHeight">Max number of vertical tiles</param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="mapOrganisation">0 for nothing placed, a 1 for a tile placed, lenght of the array must be equal to mapWidth * mapHeight</param>
        public Map(Vector2 position, int tileWidth, int tileHeight, int[,] mapOrganisation)
        {
            Data = new MapData();
            Width = mapOrganisation.GetLength(1) * tileWidth;
            Height = mapOrganisation.GetLength(0) * tileHeight;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            MapOrganisation = mapOrganisation;

            for(int y = 0; y < mapOrganisation.GetLength(0); y++)
            {
                for(int x = 0; x < mapOrganisation.GetLength(1); x++)
                {
                    if (mapOrganisation[y, x] == 1)
                    {
                        Data.RenderedEntities.Add(new SolidTile(Drawing.pointTexture, new Vector2(position.X + x * tileWidth, position.Y + y * tileHeight), tileWidth, tileHeight));
                        Data.Solids.Add(new SolidTile(Drawing.pointTexture, new Vector2(position.X + x * tileWidth, position.Y + y * tileHeight), tileWidth, tileHeight));
                    } 
                }
            }
        }

        public void LoadMap()
        {
            currentLevel = new Level(1, Vector2.Zero, this);
            currentLevel.Load();
        }

        public void Update()
        {
            for (int i = Data.RenderedEntities.Count - 1; i >= 0; i--)
                Data.RenderedEntities[i].Update();
        }

        public void Render()
        {
            for (int i = Data.RenderedEntities.Count - 1; i >= 0; i--)
                Data.RenderedEntities[i].Render();
        }

        public RenderedEntity Instantiate(RenderedEntity entity)
        {
            Data.RenderedEntities.Add(entity);
            return entity;
        }

        public void Destroy(RenderedEntity entity)
        {
            Data.RenderedEntities.Remove(entity);
        }
    }
}