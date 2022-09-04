using Fiourp;
using LDtk;
using LDtk.JsonPartials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public static class Levels
    {
        /// <summary>
        /// The first GUID corresponds to the level, the second correspond to the objects which don't respawn
        /// </summary>
        public static List<Guid> LevelNonRespawn = new();
        public static int LevelIndex;
        public static LDtkLevel LastLDtkLevel;
        public static LevelData LastLevelData { 
            get  {
                if (LastLDtkLevel != null)
                    return GetLevelData(LastLDtkLevel);
                else
                    return GetLevelData(LevelIndex);
            }
        }

        public static NineSliceSimple FallingPlatformNineSlice1
            = new NineSliceSimple(T1Cropped(120, 0), T1Cropped(136, 0), T1Cropped(120, 16), T1Cropped(136, 16), T1Cropped(128, 0), T1Cropped(120, 8), T1Cropped(136, 8), T1Cropped(128, 16), T1Cropped(128, 8), true);

        public static Texture2D T1Cropped(int posX, int posY)
            => DataManager.Textures["Tilesets/industrialAll"].CropTo(new Vector2(posX, posY), new Vector2(8));

        private static List<Entity> GetLevelEntities(this LDtkLevel level)
        {
            Random levelRandom = new Random(level.Position.X + level.Position.Y);
            Rectangle oldCamBounds = Engine.Cam.Bounds;
            Vector2 oldCamPos = Engine.Cam.CenteredPos;
            Engine.Cam.SetBoundaries(new Rectangle(level.Position, level.Size));

            List<Entity> entities = new List<Entity>();
            LDtkIntGrid intGrid = level.GetIntGrid("IntGrid");
            Dictionary<Entity, List<Guid>> IidsChildren = new();

            foreach (LDtkTypes.Platform p in level.GetEntities<LDtkTypes.Platform>())
            {
                Entity plat;
                if (p.Positions.Length == 0)
                    plat = new SolidPlatform(p.Position, p.Width(), p.Height(), FallingPlatformNineSlice1);
                else
                    plat = new CyclingPlatform(p.Position, p.Width(), p.Height(), new Sprite(FallingPlatformNineSlice1), p.GoingForwards, ArrayCenteredToTile(p.Positions), p.TimeBetweenPositions, Ease.QuintInAndOut);
                    
                entities.Add(plat);

                if (p.Children.Length != 0)
                    IidsChildren[plat] = new();

                foreach (EntityRef child in p.Children)
                    IidsChildren[plat].Add(child.EntityIid);

                /*switch (p.SpikeDirection)
                {
                    case LDtkTypes.Direction.Up:
                        plat.AddChild(new SpikeRow(new Vector2(0, -Spike.DefaultSize) + plat.Pos, Direction.Right, plat.Width, Direction.Up));
                        break;
                    case LDtkTypes.Direction.Down:
                        plat.AddChild(new SpikeRow(new Vector2(0, plat.Height) + plat.Pos, Direction.Right, plat.Width, Direction.Down));
                        break;
                    case LDtkTypes.Direction.Left:
                        plat.AddChild(new SpikeRow(new Vector2(-Spike.DefaultSize, 0) + plat.Pos, Direction.Down, plat.Height, Direction.Left));
                        break;
                    case LDtkTypes.Direction.Right:
                        plat.AddChild(new SpikeRow(new Vector2(plat.Width, 0) + plat.Pos, Direction.Down, plat.Height, Direction.Right));
                        break;
                }*/
            }

            foreach (LDtkTypes.GrapplingPoint p in level.GetEntities<LDtkTypes.GrapplingPoint>())
            {
                if (p.Positions.Length == 0)
                    entities.Add(new SwingingPoint(p.Position));
                else
                    entities.Add(new SwingingPoint(p.Position, ArrayCenteredToTile(p.Positions), p.TimeBetweenPositions, p.GoingForwards, Ease.QuintInAndOut));
            }

            foreach (LDtkTypes.FallingPlatform p in level.GetEntities<LDtkTypes.FallingPlatform>())
            {
                FallingPlatform plat = new FallingPlatform(p.Position, p.Width(), p.Height(), p.Respawning, FallingPlatformNineSlice1);
                entities.Add(plat);

                if (p.Children.Length != 0)
                    IidsChildren[plat] = new();

                foreach (EntityRef child in p.Children)
                    IidsChildren[plat].Add(child.EntityIid); 
            }
                

            foreach (LDtkTypes.RailedPulledBlock p in level.GetEntities<LDtkTypes.RailedPulledBlock>())
                entities.Add(new RailedPullBlock(p.RailPositions, p.Position, p.Width(), p.Height()));

            foreach (LDtkTypes.RespawnArea p in level.GetEntities<LDtkTypes.RespawnArea>())
                entities.Add(new RespawnTrigger(p.Position, p.Size, p.RespawnPoint));

            foreach (LDtkTypes.Spike p in level.GetEntities<LDtkTypes.Spike>())
            {
                SpikeRow spike = new SpikeRow(p.Position, p.GetDirection(), p.Length(), p.Direction.ToDirection());
                bool isChild = false;
                foreach (KeyValuePair<Entity, List<Guid>> parents in IidsChildren)
                    foreach (Guid guid in parents.Value)
                        if (p.Iid == guid)
                        {
                            parents.Key.AddChild(spike);
                            isChild = true;
                        }
                if(!isChild)
                    entities.Add(spike);
            }

            foreach (LDtkTypes.DeathTrigger p in level.GetEntities<LDtkTypes.DeathTrigger>())
                entities.Add(new DeathTrigger(p.Position, p.Size));

            foreach (LDtkTypes.Fire p in level.GetEntities<LDtkTypes.Fire>())
                entities.Add(new Fire(p.Position, p.Size, p.Direction.ToDirection()));

            foreach (LDtkTypes.JumpThru p in level.GetEntities<LDtkTypes.JumpThru>())
                entities.Add(new JumpThru(p.Position, p.Width(), p.Height(), "industrial2"));
            foreach (LDtkTypes.JumpThru2 p in level.GetEntities<LDtkTypes.JumpThru2>())
                entities.Add(new JumpThru(p.Position, p.Width(), p.Height(), "industrial2"));

            if (Engine.Player == null)
                foreach (LDtkTypes.InitPlayerSpawn p in level.GetEntities<LDtkTypes.InitPlayerSpawn>())
                    Engine.CurrentMap.Instantiate(new Player(p.Position));

            foreach (LDtkTypes.SwingTriggered p in level.GetEntities<LDtkTypes.SwingTriggered>())
            {
                SwingTriggered.Types speed = SwingTriggered.Types.Normal;
                switch (p.Speed)
                {
                    case LDtkTypes.Speed.Slow:
                        speed = SwingTriggered.Types.Slow;
                        break;
                    case LDtkTypes.Speed.Normal:
                        speed = SwingTriggered.Types.Normal;
                        break;
                    case LDtkTypes.Speed.Fast:
                        speed = SwingTriggered.Types.Fast;
                        break;
                    case LDtkTypes.Speed.VeryFast:
                        speed = SwingTriggered.Types.VeryFast;
                        break;
                }

                entities.Add(new SwingTriggered(p.Position, p.Positions, p.Width(), p.Height(), speed));
            }
                
            foreach (LDtkTypes.TextSpawn p in level.GetEntities<LDtkTypes.TextSpawn>())
                entities.Add(new TextSpawn(p.Position, p.Size, p.TextPos, p.Text));

            foreach (LDtkTypes.JetpackBooster p in level.GetEntities<LDtkTypes.JetpackBooster>())
                entities.Add(new JetpackBooster(p.Position, p.Size, p.Direction.ToDirection()));

            foreach (LDtkTypes.Refill p in level.GetEntities<LDtkTypes.Refill>())
                entities.Add(new Refill(p.Position, p.RespawnTime));

            foreach (LDtkTypes.Collectable p in level.GetEntities<LDtkTypes.Collectable>())
                if((!LevelNonRespawn.Contains(p.Iid))
                    && (!Engine.CurrentMap.Data.EntitiesByType.ContainsKey(typeof(RAM)) ||
                    Engine.CurrentMap.Data.EntitiesByType[typeof(RAM)].TrueForAll((collected) => ((RAM)collected).iid != p.Iid)))
                    entities.Add(new RAM(p.Position, p.Iid));

            foreach (LDtkTypes.Key p in level.GetEntities<LDtkTypes.Key>())
                if ((!LevelNonRespawn.Contains(p.Iid))
                    && (!Engine.CurrentMap.Data.EntitiesByType.TryGetValue(typeof(Key), out var keys) ||keys.TrueForAll((collected) => ((RAM)collected).iid != p.Iid)))
                {
                    foreach (LDtkTypes.Cage c in level.GetEntities<LDtkTypes.Cage>())
                    {
                        if(c.Iid == p.Cage.EntityIid)
                        {
                            Cage cage = new Cage(c.Position, c.Width(), c.Height());
                            entities.Add(cage);
                            entities.Add(new Key(p.Position, cage, p.Iid));
                            break;
                        }
                    }
                }

            foreach (LDtkTypes.GlassWall p in level.GetEntities<LDtkTypes.GlassWall>())
                entities.Add(new GlassWall(p.Position, p.Width(), p.Height(), p.BreakVelocity));

            foreach (LDtkTypes.CameraLock p in level.GetEntities<LDtkTypes.CameraLock>())
                entities.Add(new CameraLock(p.Position, p.Size));

            foreach (LDtkTypes.Sawblade p in level.GetEntities<LDtkTypes.Sawblade>())
            {
                if (p.Positions.Length != 0)
                    entities.Add(new Sawblade(p.Position, p.Width() / 2, ArrayCenteredToTile(p.Positions), p.TimesBetweenPositions, p.GoingForwards));
                else
                    entities.Add(new Sawblade(p.Position, p.Width() / 2));
            }

            foreach (LDtkTypes.SpecialTrigger p in level.GetEntities<LDtkTypes.SpecialTrigger>())
            {
                switch (p.Index)
                {
                    case 1:
                        entities.Add(new JetpackActivator(p.Position, p.Size));
                        break;
                }
            }

            bool downNeighbours = false;
            List<Rectangle> downNeighboursRect = new();

            foreach (NeighbourLevel n in level._Neighbours)
            {
                LDtkLevel neigh = Platformer.World.LoadLevel(n.LevelIid);
                Rectangle lvlRect = new Rectangle(level.Position, level.Size);
                Rectangle neighRect = new Rectangle(neigh.Position, neigh.Size);

                if (neighRect.X + neighRect.Width == lvlRect.X)
                {
                    //left
                    Vector2 pos = new Vector2(level.WorldX - 1, Math.Max(level.WorldY, neigh.WorldY));
                    Vector2 size;
                    if (level.WorldY + level.Size.Y <= neigh.WorldY + neigh.Size.Y)
                        size = new Vector2(2, level.WorldY + level.Size.Y - pos.Y);
                    else
                        size = new Vector2(2, neigh.WorldY + neigh.Size.Y - pos.Y);
                    entities.Add(new LevelTransition(pos, size, neigh, Direction.Left));

                }
                else if (neighRect.Y + neighRect.Height == lvlRect.Y)
                {
                    //top
                    Vector2 pos = new Vector2(Math.Max(level.WorldX, neigh.WorldX), level.WorldY - 1);
                    Vector2 size;
                    if (level.WorldX + level.Size.X < neigh.WorldX + neigh.Size.X)
                        size = new Vector2(level.WorldX + level.Size.X - neigh.WorldX, 2);
                    else
                        size = new Vector2(neigh.WorldX + neigh.Size.X - level.WorldX, 2);
                    entities.Add(new LevelTransition(pos, size, neigh, Direction.Up));
                }
                else if (neighRect.X == lvlRect.X + lvlRect.Width)
                {
                    //right
                    Vector2 pos = new Vector2(level.WorldX + level.Size.X - 1, Math.Max(level.WorldY, neigh.WorldY));
                    Vector2 size;
                    if (level.WorldY + level.Size.Y <= neigh.WorldY + neigh.Size.Y)
                        size = new Vector2(2, level.WorldY + level.Size.Y - pos.Y);
                    else
                        size = new Vector2(2, neigh.WorldY + neigh.Size.Y - pos.Y);
                    entities.Add(new LevelTransition(pos, size, neigh, Direction.Right));
                }
                else
                {
                    //bottom
                    downNeighbours = true;
                    Vector2 pos = new Vector2(Math.Max(level.WorldX, neigh.WorldX), level.WorldY + level.Size.Y - 1);

                    Vector2 size;

                    if (level.WorldX + level.Size.X < neigh.WorldX + neigh.Size.X)
                        size = new Vector2(level.WorldX + level.Size.X - neigh.WorldX, 2);
                    else
                        size = new Vector2(neigh.WorldX + neigh.Size.X - level.WorldX, 2);
                    
                    downNeighboursRect.Add(new Rectangle(neigh.Position, neigh.Size));
                    entities.Add(new LevelTransition(pos, size, neigh, Direction.Down));
                }
            }

            foreach (LayerInstance l in level.LayerInstances)
            {
                if (l._Type == LayerType.Tiles)
                {
                    string tileSet = System.IO.Path.ChangeExtension(l._TilesetRelPath, null);
                    if (!DataManager.Textures.ContainsKey(tileSet + "{X:0 Y:0}"))
                    {
                        Texture2D tileSetTex = DataManager.Load(tileSet);
                        Dictionary<Point, Texture2D> tiles = DataManager.GetTileSetTextures(tileSetTex, l._GridSize);
                        foreach (KeyValuePair<Point, Texture2D> tile in tiles)
                            DataManager.Textures[tileSet + tile.Key] = tile.Value;
                    }
                    for (int i = l.GridTiles.Length - 1; i >= 0; i--)
                    {
                        TileInstance t = l.GridTiles[i];
                        Texture2D texture = DataManager.Textures[tileSet + t.Src];
                        texture.Name = tileSet + t.T.ToString();
                        Sprite s = new Sprite(texture);
                        s.Effect = (SpriteEffects)t.F;
                        entities.Add(new Tile(new Vector2(t.Px.X + l._PxTotalOffsetX + level.Position.X, t.Px.Y + l._PxTotalOffsetY + level.Position.Y), l._GridSize, l._GridSize, s));
                    }
                }
                /*else if (l._Type == LayerType.IntGrid)
                {
                    string tileSet = System.IO.Path.ChangeExtension(l._TilesetRelPath, null);
                    if (!DataManager.Textures.ContainsKey(tileSet + "{X:0 Y:0}"))
                    {
                        Texture2D tileSetTex = DataManager.Load(tileSet);
                        Dictionary<Point, Texture2D> tiles = DataManager.GetTileSetTextures(tileSetTex, l._GridSize);
                        foreach (KeyValuePair<Point, Texture2D> tile in tiles)
                            DataManager.Textures[tileSet + tile.Key] = tile.Value;
                    }

                    foreach (TileInstance t in l.AutoLayerTiles)
                    {
                        Texture2D texture = DataManager.Textures[tileSet + t.Src];
                        texture.Name = tileSet + t.T.ToString();

                        Sprite s = new Sprite(texture);
                        s.Effect = (SpriteEffects)t.F;
                        entities.Add(new SolidTile(new Vector2(t.Px.X + l._PxTotalOffsetX + level.Position.X, t.Px.Y + l._PxTotalOffsetY + level.Position.Y), l._GridSize, l._GridSize, s));
                    }
                }*/
            }

            if (!downNeighbours)
                entities.Add(new DeathTrigger(level.Position.ToVector2() + level.Size.ToVector2().OnlyY(), new Vector2(level.Size.X, intGrid.TileSize)));
            else
            {
                bool inside = false;
                for (int x = level.Position.X; x < level.Position.X + level.PxWid; x++)
                {
                    foreach (Rectangle rect in downNeighboursRect)
                    {
                        if (x >= rect.X && x < rect.Right)
                        {
                            inside = true;
                            x = rect.Right - 1;
                            break;
                        }
                    }

                    if (!inside)
                    {
                        int minX = int.MaxValue;
                        foreach (Rectangle rect in downNeighboursRect)
                        {
                            if (rect.X < minX && rect.X > x)
                                minX = rect.X;
                        }

                        if (minX == int.MaxValue)
                            minX = x + 1;
                        
                        entities.Add(new DeathTrigger(new Vector2(x, level.Position.Y + level.Size.Y), new Vector2(minX - x, intGrid.TileSize)));
                    }
                    
                    inside = false;
                }
            }

            foreach (LDtkTypes.StreetLight p in level.GetEntities<LDtkTypes.StreetLight>())
            {
                if(p.TriggerTopLeft is Vector2 v)
                    entities.Add(new StreetLight(p.Position, p.Width(), p.Height(), new Rectangle(p.TriggerTopLeft.Value.ToPoint(), p.TriggerBottomRight.Value.ToPoint() - p.TriggerTopLeft.Value.ToPoint())));
                else
                    entities.Add(new StreetLight(p.Position, p.Width(), p.Height()));
            }

            foreach (LDtkTypes.ProjectorLight p in level.GetEntities<LDtkTypes.ProjectorLight>())
                entities.Add(new ProjectorLight(p.Position, p.Direction - p.Position, p.Range));

            Engine.Cam.SetBoundaries(oldCamBounds);
            Engine.Cam.CenteredPos = oldCamPos;

            return entities;

            Vector2[] ArrayCenteredToTile(Vector2[] v)
            {
                Vector2[] w = new Vector2[v.Length];
                for (int i = 0; i < v.Length; i++)
                    w[i] = (CenteredToTile(v[i]));
                return w;
            }

            Vector2 CenteredToTile(Vector2 v)
            {
                v -= new Vector2(intGrid.TileSize / 2);
                return v;
            }

            //Texture2D RandomTile(string id) => DataManager.GetRandomTilesetTexture(DataManager.Tilesets[1], id, levelRandom);
        }

        public static int[,] GetWorldGrid(LDtkWorld world, out Vector2 position)
        {
            int minx = int.MaxValue;
            int maxx = int.MinValue;
            int miny = int.MaxValue;
            int maxy = int.MinValue;
            int gridSize = world.LoadLevel(0).GetIntGrid("IntGrid").TileSize;

            foreach (LDtkLevel level in world.Levels)
            {
                if(level.WorldX < minx)
                    minx = level.WorldX;
                if(level.WorldY < miny)
                    miny = level.WorldY;
                if(level.WorldX + level.PxWid > maxx)
                    maxx = level.WorldX + level.PxWid;
                if(level.WorldY + level.PxHei > maxy)
                    maxy = level.WorldY + level.PxHei;
            }

            int[,] grid = new int[(maxy - miny) / gridSize, (maxx - minx) / gridSize];
            foreach(LDtkLevel level in world.Levels)
            {
                var intg = SwitchXAndY(level.GetIntGrid("IntGrid"));
                for (int x = 0; x < level.PxWid / gridSize; x++)
                    for(int y = 0; y < level.PxHei / gridSize; y++)
                    {
                        if(intg[y, x] != 0)
                            grid[(level.WorldY - miny) / gridSize + y, (level.WorldX - minx) / gridSize + x] = 1;
                    }
            }

            position = new Vector2(minx, miny);
            return grid;
        }

        public static Sprite[,] GetWorldTileSprites(LDtkWorld world, Vector2 gridPos, int[,] worldOrganisation)
        {
            Sprite[,] sprites = new Sprite[worldOrganisation.GetLength(0) + 1, worldOrganisation.GetLength(1)];
            foreach(LDtkLevel level in world.Levels)
            {
                foreach (LayerInstance l in level.LayerInstances)
                {
                    if(l._Type == LayerType.IntGrid)
                    {
                        string tileSet = System.IO.Path.ChangeExtension(l._TilesetRelPath, null);
                        if (!DataManager.Textures.ContainsKey(tileSet + "{X:0 Y:0}"))
                        {
                            Texture2D tileSetTex = DataManager.Load(tileSet);
                            Dictionary<Point, Texture2D> tiles = DataManager.GetTileSetTextures(tileSetTex, l._GridSize);
                            foreach (KeyValuePair<Point, Texture2D> tile in tiles)
                                DataManager.Textures[tileSet + tile.Key] = tile.Value;
                        }

                        foreach (TileInstance t in l.AutoLayerTiles)
                        {
                            Texture2D texture = DataManager.Textures[tileSet + t.Src];
                            texture.Name = tileSet + t.T.ToString();

                            Sprite s = new Sprite(texture);
                            s.Effect = (SpriteEffects)t.F;

                            sprites[(t.Px.Y + l._PxTotalOffsetY + level.Position.Y - (int)gridPos.Y) / l._GridSize, (t.Px.X + l._PxTotalOffsetX + level.Position.X - (int)gridPos.X) / l._GridSize] = s;
                        }
                    }
                }
            }

            return sprites;
        }

        /// <summary>
        /// Get LDTK level data
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static LevelData GetLevelData(int index, Vector2? position = null)
        {
            LevelIndex = index;
            LDtkLevel ldtk = GetLdtkLevel(index);
            if (ldtk != null)
            {
                LastLDtkLevel = ldtk;
                return GetLevelData(ldtk);
            }

            if (position == null)
                throw new Exception("Must Specify a Position for a Hard Coded Level");

            Vector2 p = (Vector2)position;
            var org = GetLevelOrganisation(index);
            return new LevelData(GetLevelEntities(index, p, GetLevelSize(org)), p, GetLevelSize(org), org, Engine.CurrentMap, GetLevelEnterAction(index), null);
        }

        public static LevelData GetLevelData(string ldtkIdentifier, Vector2? position = null)
        {
            LDtkLevel ldtk = GetLdtkLevel(ldtkIdentifier);

            if (ldtk == null)
                throw new Exception("Ldtk level with specified identifier not found");

            LastLDtkLevel = ldtk;
            return GetLevelData(ldtk);
            
        }

        public static LevelData GetLevelData(LDtkLevel ldtk)
        {
            LastLDtkLevel = ldtk;
            return new LevelData(ldtk.GetLevelEntities(), ldtk.Position.ToVector2(), ldtk.Size.ToVector2(), SwitchXAndY(ldtk.GetIntGrid("IntGrid")), Engine.CurrentMap, null, null);
        }

        public static LDtkLevel GetLdtkLevel(int index)
        {
            try {
                return Platformer.World.LoadLevel($"World_Level_{index}");
            }
            catch { 
                return null; }
        }


        public static LDtkLevel GetLdtkLevel(string id)
        {
            try { return Platformer.World.LoadLevel(id); }
            catch { return null; }
        }

        public static LevelData GetLevelData(int index, Vector2 position, int tileSize)
        {
            var org = GetLevelOrganisation(index);
            return new LevelData(GetLevelEntities(index, position, GetLevelSize(org, tileSize)), position, GetLevelSize(org, tileSize, tileSize), org, Engine.CurrentMap, GetLevelEnterAction(index), null);
        }

        public static LevelData GetLevelData(int index, Vector2 position, int tileWidth, int tileHeight)
        {
            var org = GetLevelOrganisation(index);
            return new LevelData(GetLevelEntities(index, position, GetLevelSize(org, tileWidth, tileHeight)), position, GetLevelSize(org, tileWidth, tileHeight), org, Engine.CurrentMap, GetLevelEnterAction(index), null);
        }

        public static void LoadWorldGrid(LDtkWorld world)
        {
            int[,] grid = GetWorldGrid(world, out Vector2 gridPos);
            Sprite[,] sp = GetWorldTileSprites(world, gridPos, grid);
            int gridSize = world.LoadLevel(LDtkTypes.Worlds.World.World_Level_0).GetIntGrid("IntGrid").TileSize;
            Engine.CurrentMap.Instantiate(new Grid(gridPos, gridSize, gridSize, grid, sp));
        }

        private static List<Entity> GetLevelEntities(int index, Vector2 position, Vector2 size)
        {
            Vector2 p = position;

            switch (index)
            {
                case 1:
                    var l = new List<Entity> {
                        new SwingingPoint(new Vector2(Engine.ScreenSize.X / 2, 50), new Vector2[] { new Vector2(Engine.ScreenSize.X / 2, 50), new Vector2(Engine.ScreenSize.X / 2 - 200, 50) }, new float[]{ 1.5f }, true, Ease.QuintInAndOut),
                        FallDeathTrigger(p, size)
                    };
                    l.AddRange(DefaultLevelTransitions(p, new Level(GetLevelData(2, p + Engine.ScreenSize.OnlyX())), null, null, null));
                    return l;

                case 2:
                    var pulled = new PulledPlatform(new Vector2(60, 200) + p, 200, 40, new Vector2(60, 200) + p + new Vector2(200, 40), 2, Color.Yellow, Ease.QuintInAndOut);
                    var trig = new GrapplingTrigger(new Vector2(60, 200) + p + new Vector2(200, 40), true, pulled.movingTime, pulled.Pull);
                    return new List<Entity> { pulled, trig,
                    new CyclingPlatform(40, 200, new Sprite(Color.YellowGreen), new Vector2[]{ Engine.ScreenSize / 2, Engine.ScreenSize / 2 + new Vector2(-200 ,0) }, new float[] { 1.5f }, Ease.QuintInAndOut),
                    new CyclingPlatform(200, 20, new Sprite(Color.YellowGreen), new Vector2[]{ Engine.ScreenSize / 2 + Engine.ScreenSize.OnlyX() * 0.1f, Engine.ScreenSize / 2 + new Vector2(0 ,-200) }, new float[] { 1.5f }, Ease.QuintInAndOut),
                    new RespawnTrigger(new Vector2(600, 200), new Vector2(200, 300), Vector2.Zero)
                    };

                case 3:
                    return new List<Entity>() { FallDeathTrigger(position, size),
                    new SwingingPoint(new Vector2(200, 20)),
                    new RailedPullBlock(new Vector2[] { new Vector2(50, 10), new Vector2(50, 50), new Vector2(100, 50) }, 1, 20, 20)
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
                    throw new Exception($"Couldn't find Level of index {index}");
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
                            rightLevel, Direction.Right));

            if (leftLevel != null)
                transitions.Add(new LevelTransition(new Vector2(-2, 0) + p, new Vector2(4, Engine.ScreenSize.Y),
                            leftLevel, Direction.Left));

            if (downLevel != null)
                transitions.Add(new LevelTransition(new Vector2(0, Engine.ScreenSize.Y - 2) + p, new Vector2(Engine.ScreenSize.X, 4),
                            downLevel, Direction.Down));

            if (upLevel != null)
                transitions.Add(new LevelTransition(new Vector2(0, -2) + p, new Vector2(Engine.ScreenSize.X, 4),
                            upLevel, Direction.Up));

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

        private static int[,] SwitchXAndY(LDtkIntGrid levelOrganisation)
        {
            int[,] switched = new int[levelOrganisation.GridSize.Y, levelOrganisation.GridSize.X];
            for (int i = 0; i < levelOrganisation.GridSize.Y; i++)
            {
                for (int j = 0; j < levelOrganisation.GridSize.X; j++)
                {
                    switched[i, j] = levelOrganisation.GetValueAt(j, i);
                }
            }
            return switched;
        }

        public static void ReloadLastLevelFetched()
        {
            Engine.CurrentMap.CurrentLevel.Unload();
            new Level(LastLevelData).LoadNoAutoTile();
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

        public static Direction ToDirection(this LDtkTypes.Direction dir)
        {
            switch (dir)
            {
                case LDtkTypes.Direction.Up:
                    return Direction.Up;
                case LDtkTypes.Direction.Down:
                    return Direction.Down;
                case LDtkTypes.Direction.Left:
                    return Direction.Left;
                case LDtkTypes.Direction.Right:
                    return Direction.Right;
                default:
                    return Direction.Null;
            }
        }

        public static void LoadLevel(this LDtkWorld world) {  }

        public static int Length(this LDtkTypes.Spike spike)
        {
            switch (spike.GetDirection())
            {
                case Direction.Up: case Direction.Down:
                    return spike.Height();
                default:
                    return spike.Width();
            }
        }

        public static Direction GetDirection(this ILDtkEntity entity)
        {
            if (entity.Width() < entity.Height())
                return Direction.Down;
            else
                return Direction.Right;
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
