using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Basic_platformer.Solids;
using Basic_platformer.Entities;
using Microsoft.Xna.Framework.Input;

namespace Basic_platformer.Mapping
{
    public class Map
    {
        public int Width;
        public int Height;
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
        public Map(Vector2 position)
        {
            Data = new MapData();
        }

        public void LoadMap()
        {
            currentLevel = new Level(1, Vector2.Zero, this);
            currentLevel.Load();
        }

        public void Update()
        {
            for (int i = Data.Entities.Count - 1; i >= 0; i--)
                Data.Entities[i].Update();

            if (Input.GetKeyDown(Keys.L))
            {
                currentLevel.Unload();
                currentLevel = new Level(currentLevel.Index, currentLevel.Pos, this);
                currentLevel.Load();
            }
        }

        public void Render()
        {
            for (int i = Data.RenderedEntities.Count - 1; i >= 0; i--)
                Data.RenderedEntities[i].Render();
        }

        public RenderedEntity Instantiate(RenderedEntity entity)
        {
            Data.Entities.Add(entity);

            if(entity is RenderedEntity)
                Data.RenderedEntities.Add(entity);

            return entity;
        }

        public void Destroy(RenderedEntity entity)
        {
            Data.RenderedEntities.Remove(entity);
        }
    }
}