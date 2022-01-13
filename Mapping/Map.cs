using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Basic_platformer
{
    public class Map
    {
        public int Width;
        public int Height;
        public List<SolidTile> solidTiles = new List<SolidTile>();
        public MapData Data;
        public Level CurrentLevel;

        public static Map CurrentMap { get => Platformer.CurrentMap; }

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
            CurrentLevel = new Level(3, Vector2.Zero, this);
            CurrentLevel.Load();
        }

        public void Update()
        {
            for (int i = Data.Entities.Count - 1; i >= 0; i--)
                Data.Entities[i].Update();

            if (Input.GetKeyDown(Keys.L))
            {
                CurrentLevel.Unload();
                CurrentLevel = new Level(CurrentLevel.Index, CurrentLevel.Pos, this);
                CurrentLevel.Load();
            }
        }

        public void Render()
        {
            for (int i = Data.Entities.Count - 1; i >= 0; i--)
                Data.Entities[i].Render();

            for (int i = Data.Triggers.Count - 1; i >= 0; i--)
                Data.Triggers[i].Render();
        }

        public Entity Instantiate(Entity entity)
        {
            Data.Entities.Add(entity);
            return entity;
        }

        public void Destroy(Entity entity)
            => Data.Entities.Remove(entity);
    }
}