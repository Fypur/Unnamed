using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_platformer
{
    public static class Levels
    {
        public static LevelData GetLevelData(int index, Vector2 position)
        {
            var org = GetLevelOrganisation(index);
            return new LevelData(GetLevelEntities(index, position, GetLevelSize(org)), position, org, Engine.CurrentMap, GetLevelEnterAction(index), null);
        }

        public static LevelData GetLevelData(int index, Vector2 position, int tileSize)
        {
            var org = GetLevelOrganisation(index);
            return new LevelData(GetLevelEntities(index, position, GetLevelSize(org, tileSize)), position, org, Engine.CurrentMap, GetLevelEnterAction(index), null);
        }

        public static LevelData GetLevelData(int index, Vector2 position, int tileWidth, int tileHeight)
        {
            var org = GetLevelOrganisation(index);
            return new LevelData(GetLevelEntities(index, position, GetLevelSize(org, tileWidth, tileHeight)), position, org, Engine.CurrentMap, GetLevelEnterAction(index), null);
        }

        private static List<Entity> GetLevelEntities(int index, Vector2 position, Vector2 size)
        {
            Vector2 p = position;

            switch (index)
            {
                case 1:
                    var l = new List<Entity> {
                        new GrapplingPoint(new Vector2[] { new Vector2(Engine.ScreenSize.X / 2, 50), new Vector2(Engine.ScreenSize.X / 2 - 200, 50) }, new float[]{ 1.5f }, Ease.QuintInAndOut),
                        FallDeathTrigger(p, size)
                    };
                    l.AddRange(DefaultLevelTransitions(p, new Level(GetLevelData(2, p + Engine.ScreenSizeX)), null, null, null));
                    return l;

                case 2:
                    var pulled = new PulledPlatform(new Vector2(60, 200) + p, 200, 40, new Vector2(60, 200) + p + new Vector2(200, 40), 2, Color.Yellow, Ease.QuintInAndOut);
                    var trig = new GrapplingTrigger(new Vector2(60, 200) + p + new Vector2(200, 40), true, pulled.movingTime, pulled.Pull);
                    return new List<Entity> { pulled, trig,
                    new CyclingPlatform(40, 200, Color.YellowGreen, new Vector2[]{ Engine.ScreenSize / 2, Engine.ScreenSize / 2 + new Vector2(-200 ,0) }, new float[] { 1.5f }, Ease.QuintInAndOut),
                    new CyclingPlatform(200, 20, Color.YellowGreen, new Vector2[]{ Engine.ScreenSize / 2 + Engine.ScreenSizeX * 0.1f, Engine.ScreenSize / 2 + new Vector2(0 ,-200) }, new float[] { 1.5f }, Ease.QuintInAndOut),
                    new RespawnTrigger(new Vector2(600, 200), new Vector2(200, 300), Vector2.Zero)
                    };

                case 3:
                    return new List<Entity>() { FallDeathTrigger(position, size),
                    new GrapplingPoint(new Vector2(200, 20)),
                    new FallingPlatform(new Vector2(150, 100), 20, 7),
                    };

                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        private static int[,] GetLevelOrganisation(int index)
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
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
                    };
                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        private static Action GetLevelEnterAction(int index)
        {
            switch (index)
            {
                case 1:
                case 2:
                case 3:
                    return null;
                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        public static Vector2 GetLevelSize(int[,] organisation)
            => new Vector2(8 * organisation.GetLength(1),
                    8 * organisation.GetLength(0));

        public static Vector2 GetLevelSize(int[,] organisation, int tileSize)
            => new Vector2(tileSize * organisation.GetLength(1),
                    tileSize * organisation.GetLength(0));

        public static Vector2 GetLevelSize(int[,] organisation, int tileWidth, int tileHeight)
            => new Vector2(tileWidth * organisation.GetLength(1),
                    tileHeight * organisation.GetLength(0));

        private static List<LevelTransition> DefaultLevelTransitions(Vector2 fromLevelPosition, Level rightLevel, Level leftLevel, Level upLevel, Level downLevel)
        {
            Vector2 p = fromLevelPosition;
            List<LevelTransition> transitions = new List<LevelTransition>();

            if (rightLevel != null)
                transitions.Add(new LevelTransition(new Vector2(Engine.ScreenSize.X - 2, 0) + p, new Vector2(4, Engine.ScreenSize.Y),
                            rightLevel, LevelTransition.Direction.Right));

            if (leftLevel != null)
                transitions.Add(new LevelTransition(new Vector2(-2, 0) + p, new Vector2(4, Engine.ScreenSize.Y),
                            leftLevel, LevelTransition.Direction.Left));

            if (downLevel != null)
                transitions.Add(new LevelTransition(new Vector2(0, Engine.ScreenSize.Y - 2) + p, new Vector2(Engine.ScreenSize.X, 4),
                            downLevel, LevelTransition.Direction.Down));

            if (upLevel != null)
                transitions.Add(new LevelTransition(new Vector2(0, -2) + p, new Vector2(Engine.ScreenSize.X, 4),
                            upLevel, LevelTransition.Direction.Up));


            return transitions;
        }

        private static DeathTrigger FallDeathTrigger(Vector2 levelPos, Vector2 levelSize)
            => new DeathTrigger(levelPos + new Vector2(0, levelSize.Y + Platformer.player.Height / 2), new Vector2(levelSize.X, 2));
    }
}

