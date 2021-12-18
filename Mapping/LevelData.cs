using Basic_platformer.Solids;
using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;
using Basic_platformer.Triggers;

namespace Basic_platformer.Mapping
{
    public static class LevelData
    {
        public static List<Entity> GetLevelData(Level level)
        {
            Vector2 p = level.Pos;

            switch (level.Index)
            {
                case 1:
                    return new List<Entity> {
                        new GrapplingPoint(new Vector2(Platformer.ScreenSize.X / 2, 200)),
                        new LevelTransition(new Vector2(Platformer.ScreenSize.X - 10, 0) + p, Platformer.ScreenSize + new Vector2(10, 0) + p,
                            level, new Level(2, Platformer.ScreenSizeX, Platformer.CurrentMap), LevelTransition.Direction.Right)
                    };
                case 2:
                    var pulled = new PulledPlatform(new Vector2(60, 200) + p, 200, 40, new Vector2(60, 200) + p + new Vector2(200, 40), 2, Ease.QuintInAndOut);
                    var trig = new GrapplingTrigger(new Vector2(60, 200) + p + new Vector2(200, 40), true, pulled.movingTime, pulled.Pull);
                    return new List<Entity> { pulled, trig };
                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        public static Vector2 GetLevelSize(int index)
        {
            switch (index)
            {
                case 1: case 2:
                    return Platformer.ScreenSize;
                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        public static int[,] GetLevelOrganisation(int index)
        {
            switch (index)
            {
                case 1:
                    return new int[,] {
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
                    };
                case 2:
                    return new int[,] {
                        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
                    };
                default:
                    throw new Exception("Couldn't find Level");
            }
        }
    }
}
