using Basic_platformer.Solids;
using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;


namespace Basic_platformer.Mapping
{
    public static class LevelData
    {
        public static List<Entity> GetLevelData(int index)
        {
            switch (index)
            {
                case 1:
                    Player player = new Player(new Vector2(Platformer.ScreenSize.X / 2, Platformer.ScreenSize.Y - 300), 32, 32);
                    return new List<Entity> { player };

                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        public static Vector2 GetLevelSize(int index)
        {
            switch (index)
            {
                case 1:
                    return Platformer.ScreenSize;
                default:
                    throw new Exception("Couldn't find Level");
            }
        }
    }
}
