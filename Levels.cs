using Fiourp;
using LDtk;
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
                    new RailedPullBlock(new Vector2[] { new Vector2(50, 10), new Vector2(50, 50), new Vector2(100, 50) }, 1, 20, 20)
                    };
                case 4:
                    return Platformer.Level.GetLevelEntities();

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
                case 4:
                    return SwitchXAndY(Platformer.Level.GetIntGrid("IntGrid").Values);

                default:
                    throw new Exception("Couldn't find Level");
            }
        }

        private static Action GetLevelEnterAction(int index)
        {
            switch (index)
            {
                default:
                    return null;
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

        private static int[,] SwitchXAndY(int[,] levelOrganisation)
        {
            int[,] switched = new int[levelOrganisation.GetLength(1), levelOrganisation.GetLength(0)];
            for (int i = 0; i < levelOrganisation.GetLength(0); i++)
            {
                for (int j = 0; j < levelOrganisation.GetLength(1); j++)
                {
                    switched[j, i] = levelOrganisation[i, j];
                }
            }
            return switched;
        }

        private static List<Entity> GetLevelEntities(this LDtkLevel level)
        {
            List<Entity> entities = new List<Entity>();
            Vector2 lpos = level.Position.ToVector2();
            Debug.Log(lpos, "19");

            foreach (LDtkTypes.Platform p in level.GetEntities<LDtkTypes.Platform>())
            {
                if (p.Positions.Length == 0)
                    entities.Add(new Platform(p.Position, p.Width(), p.Height(), p.Color));
                else
                    entities.Add(new CyclingPlatform(p.Width(), p.Height(), p.Color, p.Positions.ToVector2().AddAtBeggining(p.Position), p.TimeBetweenPositions, Ease.QuintInAndOut));
            }

            foreach (LDtkTypes.GrapplingPoint p in level.GetEntities<LDtkTypes.GrapplingPoint>())
            {
                if (p.Positions.Length == 0)
                    entities.Add(new GrapplingPoint(p.Position));
                else
                    entities.Add(new GrapplingPoint(p.Positions.ToVector2().AddAtBeggining(p.Position), p.TimeBetweenPositions, Ease.QuintInAndOut));
                Debug.Log("gp pos " + p.Position);
            }

            foreach (LDtkTypes.FallingPlatform p in level.GetEntities<LDtkTypes.FallingPlatform>())
                entities.Add(new FallingPlatform(p.Position, p.Width(), p.Height()));

            foreach (LDtkTypes.RailedPulledBlock p in level.GetEntities<LDtkTypes.RailedPulledBlock>())
                entities.Add(new RailedPullBlock(p.RailPositions.ToVector2(), p.Position, p.Width(), p.Height()));

            foreach (LDtkTypes.RespawnArea p in level.GetEntities<LDtkTypes.RespawnArea>())
                entities.Add(new RespawnTrigger(p.Position, p.Size, p.RespawnPoint.ToVector2()));

            foreach (LDtkTypes.Spike p in level.GetEntities<LDtkTypes.Spike>())
                entities.Add(new SpikeRow(p.Position, p.GetDirection(), p.Length(), p.Direction.ToSpikeDirection()));

                /*foreach(ILDtkEntity entity in level.GetAllEntities())
                {
                    switch (entity)
                    {
                        case LDtkTypes.Platform p:
                            entities.Add(new Platform(p.Position, p.Width(), p.Height(), p.Color))
                            break;
                    }
                }*/

                return entities;
        }

        private static int Width(this ILDtkEntity entity)
            => (int)entity.Size.X;

        private static int Height(this ILDtkEntity entity)
            => (int)entity.Size.Y;

        public static T[] AddAtBeggining<T>(this T[] array, T element)
        {
            T[] result = new T[array.Length + 1];
            result[0] = element;

            for (int i = 0; i < array.Length; i++)
                result[i + 1] = array[i];

            return result;
        }

        public static Vector2[] ToVector2(this Point[] points)
        {
            Vector2[] result = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
                result[i] = points[i].ToVector2();
            return result;
        }

        public static SpikeRow.Direction ToSpikeDirection(this LDtkTypes.Direction dir)
        {
            switch (dir)
            {
                case LDtkTypes.Direction.Up:
                    return SpikeRow.Direction.Up;
                case LDtkTypes.Direction.Down:
                    return SpikeRow.Direction.Down;
                case LDtkTypes.Direction.Left:
                    return SpikeRow.Direction.Left;
                default:
                    return SpikeRow.Direction.Right;
            }
        }

        public static int Length(this LDtkTypes.Spike spike)
        {
            switch (spike.GetDirection())
            {
                case SpikeRow.Direction.Up: case SpikeRow.Direction.Down:
                    return spike.Height();
                default:
                    return spike.Width();
            }
        }

        public static SpikeRow.Direction GetDirection(this LDtkTypes.Spike spike)
        {
            if (spike.Width() < spike.Height())
                return SpikeRow.Direction.Down;
            else
                return SpikeRow.Direction.Right;
        }

        public static Vector2[] Addition(this Vector2[] array, Vector2 addedVector)
        {
            Vector2[] result = new Vector2[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = array[i] + addedVector;
            return result;
        }
    }
}
