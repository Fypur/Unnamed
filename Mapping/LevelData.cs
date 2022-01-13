using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public static class LevelData
    {
        public static List<Entity> GetLevelData(Level level)
        {
            Vector2 p = level.Pos;

            switch (level.Index)
            {
                case 1:
                    var l = new List<Entity> {
                        new GrapplingPoint(new Vector2[] { new Vector2(Platformer.ScreenSize.X / 2, 50), new Vector2(Platformer.ScreenSize.X / 2 - 200, 50) }, new float[]{ 1.5f }, Ease.QuintInAndOut),
                        FallDeathTrigger(level)
                    };
                    l.AddRange(DefaultLevelTransitions(level, new Level(2, p + Platformer.ScreenSizeX, level.ParentMap), null, null, null));
                    return l;

                case 2:
                    var pulled = new PulledPlatform(new Vector2(60, 200) + p, 200, 40, new Vector2(60, 200) + p + new Vector2(200, 40), 2, Color.Yellow, Ease.QuintInAndOut);
                    var trig = new GrapplingTrigger(new Vector2(60, 200) + p + new Vector2(200, 40), true, pulled.movingTime, pulled.Pull);
                    return new List<Entity> { pulled, trig,
                    new CyclingPlatform(40, 200, Color.YellowGreen, new Vector2[]{ Platformer.ScreenSize / 2, Platformer.ScreenSize / 2 + new Vector2(-200 ,0) }, new float[] { 1.5f }, Ease.QuintInAndOut),
                    new CyclingPlatform(200, 20, Color.YellowGreen, new Vector2[]{ Platformer.ScreenSize / 2 + Platformer.ScreenSizeX * 0.1f, Platformer.ScreenSize / 2 + new Vector2(0 ,-200) }, new float[] { 1.5f }, Ease.QuintInAndOut),
                    new RespawnTrigger(new Vector2(600, 200), new Vector2(200, 300), Vector2.Zero)
                    };

                case 3:
                    return new List<Entity>() { FallDeathTrigger(level),
                    new GrapplingPoint(new Vector2(200, 100)),
                    new SpikeRow(new Vector2(90, Platformer.ScreenSize.Y - 25), SpikeRow.Direction.Up, 200, SpikeRow.Direction.Left)
                    };

                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        public static Vector2 GetLevelSize(Level level)
            => new Vector2(level.TileWidth * level.Organisation.GetLength(1),
                    level.TileHeight * level.Organisation.GetLength(0));

        public static int[,] GetLevelOrganisation(int index)
        {
            switch (index)
            {
                case 1:
                    return new int[,] {
                        {1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1}
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
                case 3:
                    return new int[,] {
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
                    };
                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        public static Action GetLevelEnterAction(int index)
        {
            switch (index)
            {
                case 1: case 2: case 3:
                    return null;
                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        public static List<LevelTransition> DefaultLevelTransitions(Level fromLevel ,Level rightLevel, Level leftLevel, Level upLevel, Level downLevel)
        {
            Vector2 p = fromLevel.Pos;

            List<LevelTransition> transitions = new List<LevelTransition>();

            if (rightLevel != null)
                transitions.Add(new LevelTransition(new Vector2(Platformer.ScreenSize.X - 2, 0) + p, new Vector2(4, Platformer.ScreenSize.Y),
                            fromLevel, rightLevel, LevelTransition.Direction.Right));

            if (leftLevel != null)
                transitions.Add(new LevelTransition(new Vector2(-2, 0) + p, new Vector2(4, Platformer.ScreenSize.Y),
                            fromLevel, leftLevel, LevelTransition.Direction.Left));

            if (downLevel != null)
                transitions.Add(new LevelTransition(new Vector2(0, Platformer.ScreenSize.Y - 2) + p, new Vector2(Platformer.ScreenSize.X, 4),
                            fromLevel, downLevel, LevelTransition.Direction.Down));

            if (upLevel != null)
                transitions.Add(new LevelTransition(new Vector2(0, -2) + p, new Vector2(Platformer.ScreenSize.X, 4),
                            fromLevel, upLevel, LevelTransition.Direction.Up));


            return transitions;
        }

        public static DeathTrigger FallDeathTrigger(Level level)
            => new DeathTrigger(level.Pos + new Vector2(0, level.Size.Y + Platformer.player.Height / 2), new Vector2(level.Size.X, 2));
    }
}
