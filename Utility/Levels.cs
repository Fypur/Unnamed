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
        /// IIds of entities that won't be respawned again
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

        public static Dictionary<int, string> TileData = new();

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
                    plat = new CyclingPlatform(p.Position, p.Width(), p.Height(), new Sprite(FallingPlatformNineSlice1), p.GoingForwards, ArrayCenteredToTile(p.Positions), p.TimeBetweenPositions, Ease.CubeInAndOut);
                    
                entities.Add(plat);

                if(p.Children.Length > 0)
                    IidsChildren[plat] = new();
                foreach (EntityRef child in p.Children)
                    IidsChildren[plat].Add(child.EntityIid);
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

            foreach (LDtkTypes.GrapplingPoint p in level.GetEntities<LDtkTypes.GrapplingPoint>())
            {
                if (p.Positions.Length == 0)
                    AddIfChild(new SwingingPoint(p.Position, p.MaxSwingDistance), p.Iid);
                else
                    AddIfChild(new SwingingPoint(p.Position, p.MaxSwingDistance, ArrayCenteredToTile(p.Positions), p.TimeBetweenPositions, p.GoingForwards, Ease.CubeInAndOut), p.Iid);
            }
                

            foreach (LDtkTypes.RailedPulledBlock p in level.GetEntities<LDtkTypes.RailedPulledBlock>())
                entities.Add(new RailedPullBlock(p.RailPositions, p.MaxSwingDistance, p.Position, p.Width(), p.Height()));

            if (Engine.Player == null)
                foreach (LDtkTypes.InitPlayerSpawn p in level.GetEntities<LDtkTypes.InitPlayerSpawn>())
                    Engine.CurrentMap.Instantiate(new Player(p.Position));

            foreach (LDtkTypes.RespawnArea p in level.GetEntities<LDtkTypes.RespawnArea>())
            {
                if(Engine.Player == null)
                    Engine.CurrentMap.Instantiate(new Player(p.RespawnPoint));

                entities.Add(new RespawnTrigger(p.Position, p.Size, p.RespawnPoint));
            }

            foreach (LDtkTypes.Spike p in level.GetEntities<LDtkTypes.Spike>())
            {
                SpikeRow spike = new SpikeRow(p.Position, p.GetDirection(), p.Length(), p.Direction.ToDirection());

                AddIfChild(spike, p.Iid);
            }

            foreach (LDtkTypes.DeathTrigger p in level.GetEntities<LDtkTypes.DeathTrigger>())
                entities.Add(new DeathTrigger(p.Position, p.Size));

            foreach (LDtkTypes.Fire p in level.GetEntities<LDtkTypes.Fire>())
                AddIfChild(new Fire(p.Position, p.Size, p.Direction.ToDirection()), p.Iid);

            foreach (LDtkTypes.JumpThru p in level.GetEntities<LDtkTypes.JumpThru>())
                entities.Add(new JumpThru(p.Position, p.Width(), p.Height(), "industrial2"));
            foreach (LDtkTypes.JumpThru2 p in level.GetEntities<LDtkTypes.JumpThru2>())
                entities.Add(new JumpThru(p.Position, p.Width(), p.Height(), "industrial2"));

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

                entities.Add(new SwingTriggered(p.Positions, p.MaxSwingDistance, p.Position, p.Width(), p.Height(), speed));
            }
                
            foreach (LDtkTypes.TextSpawn p in level.GetEntities<LDtkTypes.TextSpawn>())
                entities.Add(new TextSpawn(p.Position, p.Size, p.TextPos + new Vector2(p.XOffset, p.YOffset), p.Color, p.Text));

            foreach (LDtkTypes.JetpackBooster p in level.GetEntities<LDtkTypes.JetpackBooster>())
                entities.Add(new JetpackBooster(p.Position, p.Size, p.Direction.ToDirection()));

            foreach (LDtkTypes.Refill p in level.GetEntities<LDtkTypes.Refill>())
                entities.Add(new FuelRefill(p.Position, p.RespawnTime));

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


            foreach (LDtkTypes.HangingWire p in level.GetEntities<LDtkTypes.HangingWire>())
                entities.Add(new HangingWire(p.ControlPoints.AddAtBeggining(p.Position)));

            foreach (LDtkTypes.GlassWall p in level.GetEntities<LDtkTypes.GlassWall>())
                entities.Add(new GlassWall(p.Position, p.SolidInDirection == null ? null : p.SolidInDirection.Value.ToDirection(), p.Width(), p.Height(), p.BreakVelocity));

            foreach (LDtkTypes.CamLock p in level.GetEntities<LDtkTypes.CamLock>())
                entities.Add(new CameraLock(p.Position, p.Size));

            foreach (LDtkTypes.CamOffset p in level.GetEntities<LDtkTypes.CamOffset>())
                entities.Add(new CameraOffset(p.Position, p.Size, p.Offset - p.Position - new Vector2(intGrid.TileSize / 2) + new Vector2(p.OffsetX, p.OffsetY), p.Override));

            foreach (LDtkTypes.CameraBlock p in level.GetEntities<LDtkTypes.CameraBlock>())
                entities.Add(new CameraBlock(p.Position, p.Size));

            foreach (LDtkTypes.InvisibleWall p in level.GetEntities<LDtkTypes.InvisibleWall>())
                entities.Add(new InvisibleWall(p.Position + new Vector2(p.OffsetX, p.OffsetY), p.Width(), p.Height()));

            foreach (LDtkTypes.Sawblade p in level.GetEntities<LDtkTypes.Sawblade>())
            {
                if (p.Positions.Length != 0)
                    entities.Add(new Sawblade(p.Position, p.Width() / 2, ArrayCenteredToTile(p.Positions), p.TimesBetweenPositions, p.GoingForwards));
                else
                    entities.Add(new Sawblade(p.Position, p.Width() / 2));
            }

            foreach (LDtkTypes.ParticleEmitter p in level.GetEntities<LDtkTypes.ParticleEmitter>())
            {
                if (Levels.LevelNonRespawn.Contains(p.Iid))
                    continue;

                ParticleType pT;

                switch (p.Type)
                {
                    case LDtkTypes.ParticleTypes.Waterfall: pT = Particles.WaterFall; break;
                    default: pT = Particles.Debug; break;
                }

                ParticleSoundEmitter e = new ParticleSoundEmitter(p.Position, pT, p.Amount, p.Direction, pT.Color, p.Iid);
                entities.Add(e);
                
                //entities.Add(new ParticleEntity(p.Position, Engine.CurrentMap.BackgroundSystem, pT, p.Amount, p.Direction, pT.Color));
            }

            foreach (LDtkTypes.Light p in level.GetEntities<LDtkTypes.Light>())
            {
                Entity light = new Entity(p.Position);
                if (p.Range == 360)
                    light.AddComponent(new CircleLight(Vector2.Zero, p.Length, new Color(p.Color, p.Opacity), new Color(p.Color, 0)));
                else
                    light.AddComponent(new ArcLight(Vector2.Zero, p.Direction, p.Range, p.Length, new Color(p.Color, p.Opacity), new Color(p.Color, 0)));

                Light l = light.GetComponent<Light>();
                l.CollideWithWalls = p.CollideWithWalls;

                if(p.BlinkTime is float b)
                    l.StartBlink(b);

                entities.Add(light);
            }

            foreach (LDtkTypes.Boss p in level.GetEntities<LDtkTypes.Boss>())
            {
                if(p.Id == 0)
                    entities.Add(new Boss(p.Position));
                else if (p.Id == 1)
                    entities.Add(new Boss2(p.Position));
                else if (p.Id == 2)
                    entities.Add(new Boss3(p.Position));
            }

            foreach (LDtkTypes.SpecialTrigger p in level.GetEntities<LDtkTypes.SpecialTrigger>())
            {
                switch (p.TypeId)
                {
                    case 0:
                        entities.Add(new IdentifierTrigger(p.Position, p.Size, p.Id));
                        break;
                    case 1:
                        entities.Add(new JetpackActivator(p.Position, p.Size));
                        break;
                    case 3:
                        List<Entity> spawned = new();
                        foreach (EntityRef entity in p.Children)
                        {
                            foreach(EntityRef child in p.Children)
                            {
                                LDtkTypes.ChaseMissile c = level.GetEntityRef<LDtkTypes.ChaseMissile>(child);
                                spawned.Add(new ChaseMissile(c.ControlPoints.AddAtBeggining(c.Position), c.Time));
                            }
                        }

                        entities.Add(new SpawnTrigger(p.Position, p.Size, spawned));
                        break;
                    case 4:
                        entities.Add(new SpawnTrigger(p.Position, p.Size, new Entity[] { new ChaseBoss(p.Positions, p.Id) }));
                        break;
                    case 5:
                        if (p.Id == 0)
                        {
                            if(Engine.CurrentMap.Data.GetEntities<PushingFire>().Count == 0)
                                entities.Add(new SpawnTrigger(p.Position, p.Size, new Entity[] { new PushingFire(level.Position.ToVector2()) }));
                            else
                            {
                                PushingFire pushingFire = Engine.CurrentMap.Data.GetEntity<PushingFire>();
                                pushingFire.Height = level.PxHei;
                            }
                        }
                        else if (p.Id == 1)
                        {
                            Trigger trig = new Trigger(p.Position, p.Size, new() { typeof(Player) }, null);

                            trig.OnTriggerEnterAction = (entity) =>
                            {
                                var e = Engine.CurrentMap.Data.GetEntities<PushingFire>();
                                for (int i = 0; i < e.Count; i++)
                                    e[i].SelfDestroy();
                            };

                            entities.Add(trig);
                        }
                        else if (p.Id == 3)
                        {
                            var e = Engine.CurrentMap.Data.GetEntities<PushingFire>();
                            for (int i = 0; i < e.Count; i++)
                            {
                                if (e[i].Direction != Direction.Up)
                                    e[i].SelfDestroy();
                            }

                            if (Engine.CurrentMap.Data.GetEntities<PushingFire>().Count == 0)
                                entities.Add(new SpawnTrigger(p.Position, p.Size, new Entity[] { new PushingFire(level.Position.ToVector2(), 32, Direction.Up) }));
                            else
                            {
                                PushingFire pushingFire = Engine.CurrentMap.Data.GetEntity<PushingFire>();
                                pushingFire.Width = level.PxWid;
                            }
                        }
                        break;
                    case 6:
                        Trigger trig2 = new Trigger(p.Position, p.Size, new() { typeof(Player) }, null);

                        bool trigged = false;
                        trig2.OnTriggerEnterAction = (entity) =>
                        {
                            if (trigged)
                                return;

                            trigged = true;
                            Vector2[] fPos = new Vector2[p.Children.Length];
                            for (int i = 0; i < p.Children.Length; i++)
                                fPos[i] = level.GetEntityRef<LDtkTypes.FallingPlatform>(p.Children[i]).Position;

                            foreach (FallingPlatform f in Engine.CurrentMap.Data.GetEntities<FallingPlatform>())
                                if (fPos.Contains(f.Pos))
                                    f.Fall();
                        };

                        entities.Add(trig2);
                        break;
                    case 7:
                        Trigger trig3 = new Trigger(p.Position, p.Size, new() { typeof(Player) }, null);

                        trig3.OnTriggerEnterAction = (entity) =>
                        {
                            var s = Engine.CurrentMap.Data.GetEntity<SwingTriggered>();

                            s.AddComponent(new Shaker(0.4f, 4, null, true));

                            trig3.AddComponent(new Timer(0.4f, true, null, () =>
                            {
                                
                                s.GravityScale = 0.7f;
                                s.Attached = false;
                            }));
                        };

                        entities.Add(trig3);
                        break;
                    case 8:
                        ClosingGate closing = new ClosingGate(p.Position, p.Width(), p.Height(), p.Iid);
                        LDtkTypes.SpecialTrigger closingPoint = level.GetEntityRef<LDtkTypes.SpecialTrigger>(p.Children[0]);

                         
                        Trigger trig4 = new Trigger(closingPoint.Position, closingPoint.Size, new() { typeof(Player) }, null);
                        bool activated = false;
                        trig4.OnTriggerEnterAction = (e) => { if(!activated)
                                closing.Close(); };
                        if (closingPoint.Id == 100)
                        {
                            if (ClosingGate.ClosedGates.TryGetValue(p.Iid, out bool closed) && closed)
                                trig4.OnTriggerEnterAction += (e) => { if (!activated) Engine.CurrentMap.Data.GetEntity<Boss3>().Start(true); };
                            else
                                trig4.OnTriggerEnterAction += (e) => { if (!activated) Engine.CurrentMap.Data.GetEntity<Boss3>().Start(false); };
                        }

                        trig4.OnTriggerEnterAction += (e) => activated = true;

                        entities.Add(closing);
                        entities.Add(trig4);
                        break;
                    case 9:
                        if(!LevelNonRespawn.Contains(p.Iid))
                            entities.Add(new JetpackPickUp(p.Position, p.Iid, p.Id));
                        break;
                    case 10:
                        entities.Add(new Ghost(p.Id));
                        break;
                    /*case 11:
                        entities.Add(new JetpackRemove(p.Position, p.Size));
                        break;*/
                }
            }

            foreach (LDtkTypes.ChapterTransition p in level.GetEntities<LDtkTypes.ChapterTransition>())
                entities.Add(new ChapterTransition(p.Position, p.Size, p.ToLevel));

            bool downNeighbours = false;
            List<Rectangle> downNeighboursRect = new();

            foreach (NeighbourLevel n in level._Neighbours)
            {
                LDtkLevel neigh = Platformer.World.LoadLevel(n.LevelIid);

                if (neigh.Identifier.Contains("Filler"))
                    continue;

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
                        size = new Vector2(level.WorldX + level.Size.X - pos.X, 2);
                    else
                        size = new Vector2(neigh.WorldX + neigh.Size.X - pos.X, 2);
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
                        size = new Vector2(level.WorldX + level.Size.X - pos.X, 2);
                    else
                        size = new Vector2(neigh.WorldX + neigh.Size.X - pos.X, 2);
                    
                    downNeighboursRect.Add(new Rectangle(neigh.Position, neigh.Size));
                    entities.Add(new LevelTransition(pos, size, neigh, Direction.Down));
                }
            }

            foreach (LayerInstance l in level.LayerInstances)
            {
                if (l._Type == LayerType.Tiles)
                {
                    if(l._TilesetRelPath.Contains("AnimatedDecals"))
                    {
                        string tileSet = System.IO.Path.ChangeExtension(l._TilesetRelPath, null);
                        List<Sprite.Animation.Slice> slices = (List<Sprite.Animation.Slice>)((object[])DataManager.Textures[tileSet.Substring("Graphics/".Length)].Tag)[1];

                        if (TileData.Count == 0)
                        {
                            TileCustomMetadata[] tilesetData = null;
                            foreach (TilesetDefinition definition in Platformer.LDtkFile.Defs.Tilesets)
                                if (definition.Uid == l._TilesetDefUid)
                                {
                                    tilesetData = definition.CustomData;
                                    break;
                                }

                            foreach(TileCustomMetadata data in tilesetData)
                                TileData[data.TileId] = data.Data;
                        }

                        foreach (TileInstance t in l.GridTiles)
                        {
                            Rectangle spriteRect = new Rectangle(t.Src.X, t.Src.Y, l._GridSize, l._GridSize);

                            if (TileData.TryGetValue(t.T, out string data))
                            {
                                Sprite s = new();
                                s.Add(Sprite.AllAnimData[data]);

                                Tile tile = new Tile(new Vector2(t.Px.X + l._PxTotalOffsetX + level.Position.X, t.Px.Y + l._PxTotalOffsetY + level.Position.Y), l._GridSize, l._GridSize, s);
                                tile.Layer = -1;

                                entities.Add(tile);
                            }

                            AttachSliceLights(l, t, slices, null, level);
                        }
                    }
                    else if (l._TilesetRelPath.Contains("Lights"))
                    {
                        string tileSet = System.IO.Path.ChangeExtension(l._TilesetRelPath, null);

                        if (!DataManager.Textures.ContainsKey(tileSet))
                            DataManager.Textures[tileSet] = DataManager.Load(tileSet);

                        List<Sprite.Animation.Slice> slices = (List<Sprite.Animation.Slice>)((object[])DataManager.Textures[tileSet].Tag)[1];

                        for (int i = l.GridTiles.Length - 1; i >= 0; i--)
                        {
                            TileInstance t = l.GridTiles[i];

                            Texture2D texture = DataManager.Textures[tileSet];
                            texture.Name = tileSet + t.T.ToString();

                            Sprite s = new Sprite(texture);
                            s.SpriteEffect = (SpriteEffects)t.F;
                            s.SourceRectangle = new Rectangle(t.Src.X, t.Src.Y, l._GridSize, l._GridSize);

                            Tile tile = new Tile(new Vector2(t.Px.X + l._PxTotalOffsetX + level.Position.X, t.Px.Y + l._PxTotalOffsetY + level.Position.Y), l._GridSize, l._GridSize, s);
                            tile.Layer = -1;

                            AttachSliceLights(l, t, slices, tile, level);

                            entities.Add(tile);
                        }
                    }
                    else
                    {
                        string tileSet = System.IO.Path.ChangeExtension(l._TilesetRelPath, null);

                        if (!DataManager.Textures.ContainsKey(tileSet))
                            DataManager.Textures[tileSet] = DataManager.Load(tileSet);

                        for (int i = l.GridTiles.Length - 1; i >= 0; i--)
                        //for (int i = 0; i < l.GridTiles.Length; i++)
                        {
                            TileInstance t = l.GridTiles[i];

                            Texture2D texture = DataManager.Textures[tileSet];
                            texture.Name = tileSet + t.T.ToString();

                            Sprite s = new Sprite(texture);
                            s.SpriteEffect = (SpriteEffects)t.F;
                            s.SourceRectangle = new Rectangle(t.Src.X, t.Src.Y, l._GridSize, l._GridSize);

                            Tile tile = new Tile(new Vector2(t.Px.X + l._PxTotalOffsetX + level.Position.X, t.Px.Y + l._PxTotalOffsetY + level.Position.Y), l._GridSize, l._GridSize, s);
                            tile.Layer = -2;

                            entities.Add(tile);
                        }
                    }
                }

                if (l._Type == LayerType.IntGrid && l._Identifier != "IntGrid")
                {

                    string tileSet = System.IO.Path.ChangeExtension(l._TilesetRelPath, null);

                    if (!DataManager.Textures.ContainsKey(tileSet))
                        DataManager.Textures[tileSet] = DataManager.Load(tileSet);


                    int layer = l._Identifier == "DecalIntGrid" ? -2 : l._Identifier == "BgTiles" ? -3 : throw new Exception("Tiles layer was not identified");

                    foreach (TileInstance t in l.AutoLayerTiles)
                    {
                        Texture2D texture = DataManager.Textures[tileSet];
                        texture.Name = tileSet + t.T.ToString();

                        Sprite s = new Sprite(texture);
                        s.SpriteEffect = (SpriteEffects)t.F;
                        s.SourceRectangle = new Rectangle(t.Src.X, t.Src.Y, l._GridSize, l._GridSize);

                        Tile tile = new Tile(new Vector2(t.Px.X + l._PxTotalOffsetX + level.Position.X, t.Px.Y + l._PxTotalOffsetY + level.Position.Y), l._GridSize, l._GridSize, s);

                        tile.Layer = layer;

                        entities.Add(tile);
                    }
                }
            }

            foreach(NeighbourLevel neighbor in level._Neighbours)
            {
                LDtkLevel neigh = Platformer.World.LoadLevel(neighbor.LevelIid);
                Entity e = new Entity(Vector2.Zero);

                if (neigh.Identifier.Contains("Filler"))
                    continue;

                foreach(LayerInstance l in neigh.LayerInstances)
                {
                    if(l._Identifier == "AnimatedTiles")
                    {
                        List<Sprite.Animation.Slice> slices = (List<Sprite.Animation.Slice>)((object[])DataManager.Textures[System.IO.Path.ChangeExtension(l._TilesetRelPath, null).Substring("Graphics/".Length)].Tag)[1];

                        foreach (TileInstance t in l.GridTiles)
                            AttachSliceLights(l, t, slices, null, neigh);
                    }
                    else if(l._Identifier == "Lights")
                    {
                        List<Sprite.Animation.Slice> slices = (List<Sprite.Animation.Slice>)((object[])DataManager.Textures[System.IO.Path.ChangeExtension(l._TilesetRelPath, null)].Tag)[1];

                        foreach (TileInstance t in l.GridTiles)
                            AttachSliceLights(l, t, slices, null, neigh);
                    }
                }

                foreach (LDtkTypes.Light p in neigh.GetEntities<LDtkTypes.Light>())
                {
                    Light l;
                    if (p.Range == 360)
                        l = (Light)e.AddComponent(new CircleLight(p.Position, p.Length, new Color(p.Color, p.Opacity), new Color(p.Color, 0)));
                    else
                        l = (Light)e.AddComponent(new ArcLight(p.Position, p.Direction, p.Range, p.Length, new Color(p.Color, p.Opacity), new Color(p.Color, 0)));

                    l.CollideWithWalls = p.CollideWithWalls;

                    if (p.BlinkTime is float b)
                        l.StartBlink(b);
                }

                foreach (LDtkTypes.StreetLight p in neigh.GetEntities<LDtkTypes.StreetLight>())
                    if(p.TriggerTopLeft == null)
                        entities.Add(new StreetLight(p.Position, p.Width(), p.Height()));

                foreach (LDtkTypes.GrapplingPoint p in neigh.GetEntities<LDtkTypes.GrapplingPoint>())
                {
                    e.AddComponent(new CircleLight(p.Position + p.Size / 2, Math.Min(p.MaxSwingDistance, 100), new Color(Color.LightBlue, 50), new Color(Color.LightBlue, 0)));
                }

                entities.Add(e);
            }

            if (!downNeighbours)
                entities.Add(new DeathTrigger(level.Position.ToVector2() + new Vector2(0, level.Size.Y + 10), new Vector2(level.Size.X, intGrid.TileSize), true));
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
                        Rectangle r = new Rectangle(0, 0, 0, 0);
                        foreach (Rectangle rect in downNeighboursRect)
                        {
                            if (rect.X < minX && rect.X > x)
                            {
                                minX = rect.X;
                                r = rect;
                            }
                        }

                        if (minX == int.MaxValue)
                            minX = level.Position.X + level.PxWid;
                        
                        entities.Add(new DeathTrigger(new Vector2(x, level.Position.Y + level.Size.Y + 10), new Vector2(minX - x, intGrid.TileSize), true));
                        x = minX + r.Width;
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

            foreach (LDtkTypes.TriggeredDecal p in level.GetEntities<LDtkTypes.TriggeredDecal>())
            {
                Trigger trig = new Trigger(new Rectangle(p.Pos1.Value.ToPoint(), p.Pos2.Value.ToPoint() - p.Pos1.Value.ToPoint()), new() { typeof(Player) }, null);

                int tileID = 0;
                EnumDefinition e = Platformer.LDtkFile.Defs.Enums[3];
                foreach(var a in e.Values)
                {
                    if(p.Type.ToString() == a.Id)
                    {
                        tileID = (int)a.TileId;
                        if (tileID == -1)
                        {
                            tileID = a._TileSrcRect[0] / 8 + a._TileSrcRect[1] / 8 * 160 / 8;
                        }

                        break;
                    }
                }

                TilesetDefinition tileset;
                foreach (TilesetDefinition definition in Platformer.LDtkFile.Defs.Tilesets)
                    if (e.IconTilesetUid == definition.Uid)
                    {
                        tileset = definition;
                        break;
                   }


                var data = TileData[tileID];

                Sprite s = new();
                s.Add(Sprite.AllAnimData[data]);

                Tile tile = new Tile(p.Position, p.Width(), p.Height(), s);
                tile.Layer = -1;

                switch (p.Type)
                {
                    case LDtkTypes.TrigDecalType.StreetLight:
                        trig.OnTriggerEnterAction += (e) => { s.Play("blink"); };
                        break;
                    case LDtkTypes.TrigDecalType.FallingMachine:
                        trig.OnTriggerEnterAction += (e) => { s.Play("falling"); };
                        break;
                }

                trig.OnTriggerEnterAction += (e) => trig.SelfDestroy();
                
                entities.Add(tile);

                entities.Add(trig);
            }

            int camWidth = ((System.Text.Json.JsonElement)level.FieldInstances[0]._Value).GetInt32();
            if(Engine.Cam.Size.X != camWidth)
                Engine.Cam.Size = new Vector2(camWidth, (int)(9 * (float)camWidth / 16));

            if (Engine.RenderTarget.Width != camWidth)
            {
                int camHeight = (int)(9 * (float)camWidth / 16);

                Engine.RenderTarget = new RenderTarget2D(Platformer.GraphicsManager.GraphicsDevice, camWidth, camHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                Engine.PrimitivesRenderTarget = new RenderTarget2D(Platformer.GraphicsManager.GraphicsDevice, camWidth, camHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                Platformer.SecondRenderTarget = new RenderTarget2D(Platformer.GraphicsManager.GraphicsDevice, camWidth, camHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            }


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

            void AddIfChild(Entity entity, Guid iid)
            {
                bool isChild = false;
                foreach (KeyValuePair<Entity, List<Guid>> parents in IidsChildren)
                    for(int i = parents.Value.Count - 1; i >= 0; i--)
                        if (iid == parents.Value[i])
                        {
                            parents.Key.AddChild(entity);
                            parents.Value.RemoveAt(i);
                            isChild = true;
                        }

                if (!isChild)
                    entities.Add(entity);
            }

            void AttachSliceLights(LayerInstance l, TileInstance t, IList<Sprite.Animation.Slice> slices, Entity attachToEntity, LDtkLevel lvl)
            {
                foreach (Sprite.Animation.Slice slice in slices)
                {
                    if (new Rectangle(t.Src.X, t.Src.Y, l._GridSize, l._GridSize).Contains(slice.Rect.Location))
                    {
                        float direction = float.Parse(slice.Name.Substring(slice.Name.LastIndexOf('D') + 1, slice.Name.IndexOf(" ", slice.Name.LastIndexOf('D')) - slice.Name.LastIndexOf('D')));
                        float length = float.Parse(slice.Name.Substring(slice.Name.LastIndexOf('L') + 1, slice.Name.IndexOf(" ", slice.Name.LastIndexOf('L')) - slice.Name.LastIndexOf('L')));

                        int lR = slice.Name.LastIndexOf('R');
                        float range;

                        if (slice.Name.LastIndexOf(" ") > lR)
                            range = float.Parse(slice.Name.Substring(lR + 1, slice.Name.IndexOf(" ", lR) - lR));
                        else
                            range = float.Parse(slice.Name.Substring(lR + 1));

                        Vector2 location;
                        Vector2 location2;

                        if (slice.Name[0] == 'F')
                        {
                            location = new Vector2(slice.Rect.X + slice.Rect.Width - t.Src.X, slice.Rect.Y - t.Src.Y);
                            location2 = new Vector2(slice.Rect.X - t.Src.X, slice.Rect.Y + slice.Rect.Height - t.Src.Y);
                        }
                        else
                        {
                            location = (slice.Rect.Location - t.Src).ToVector2();
                            location2 = (slice.Rect.Location - t.Src + slice.Rect.Size).ToVector2();
                        }

                        if (attachToEntity == null)
                        {
                            attachToEntity = new Entity(new Vector2(t.Px.X + l._PxTotalOffsetX + lvl.Position.X, t.Px.Y + l.PxOffsetY + lvl.Position.Y));
                            entities.Add(attachToEntity);
                        }


                        if (range == 360)
                        {
                            Light l2 = (Light)attachToEntity.AddComponent(new CircleLight(location, length, new Color(slice.Color, 255), new Color(slice.Color, 0)));
                            l2.CollideWithWalls = false;
                        }
                        else
                            attachToEntity.AddComponent(new QuadPointLight(location, location2, direction, range, length, new Color(slice.Color, 255), new Color(slice.Color, 0)));
                    }
                }
            }

            //Texture2D RandomTile(string id) => DataManager.GetRandomTilesetTexture(DataManager.Tilesets[1], id, levelRandom);
        }

        public static int[,] GetWorldGrid(LDtkWorld world, int worldDepth, out Vector2 position)
        {
            int minx = int.MaxValue;
            int maxx = int.MinValue;
            int miny = int.MaxValue;
            int maxy = int.MinValue;
            int gridSize = world.LoadLevel(0).GetIntGrid("IntGrid").TileSize;

            foreach (LDtkLevel level in world.Levels)
            {
                if (level.WorldDepth != worldDepth)
                    continue;

                if (level.WorldX < minx)
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
                if (level.WorldDepth != worldDepth)
                    continue;

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

        public static Sprite[,] GetWorldTileSprites(LDtkWorld world, int worldDepth, Vector2 gridPos, int[,] worldOrganisation)
        {
            Sprite[,] sprites = new Sprite[worldOrganisation.GetLength(0) + 1, worldOrganisation.GetLength(1)];
            foreach(LDtkLevel level in world.Levels)
            {
                if (level.WorldDepth != worldDepth)
                    continue;

                foreach (LayerInstance l in level.LayerInstances)
                {
                    if(l._Type == LayerType.IntGrid && !l._Identifier.Contains("Decal") && !l._Identifier.Contains("Bg") && !l._Identifier.Contains("Tiles"))
                    {
                        string tileSet = System.IO.Path.ChangeExtension(l._TilesetRelPath, null);
                        if (!DataManager.Textures.ContainsKey(tileSet))
                            DataManager.Textures[tileSet] = DataManager.Load(tileSet);

                        foreach (TileInstance t in l.AutoLayerTiles)
                        {
                            Texture2D texture = DataManager.Textures[tileSet];
                            texture.Name = tileSet + t.T.ToString();

                            Sprite s = new Sprite(texture);
                            s.SpriteEffect = (SpriteEffects)t.F;
                            s.SourceRectangle = new Rectangle(t.Src.X, t.Src.Y, l._GridSize, l._GridSize);

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
            return new LevelData(GetLevelEntities(index, p, GetLevelSize(org)), p, GetLevelSize(org), org, Engine.CurrentMap, GetLevelEnterAction(index));
        }

        public static LevelData GetLevelData(string ldtkIdentifier)
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
            return new LevelData(ldtk.GetLevelEntities(), ldtk.Position.ToVector2(), ldtk.Size.ToVector2(), SwitchXAndY(ldtk.GetIntGrid("IntGrid")), Engine.CurrentMap, null);
        }

        public static LDtkLevel GetLdtkLevel(int index)
        {
            try {
                return Platformer.World.LoadLevel($"Lvl{index}");
            }
            catch {
                try
                {
                    return Platformer.World.LoadLevel(index);
                }
                catch
                {
                    return null; 
                }
            }
        }


        public static LDtkLevel GetLdtkLevel(string id)
        {
            try
            {
                return Platformer.World.LoadLevel($"Lvl{id}");
            }
            catch
            {
                try
                {
                    return Platformer.World.LoadLevel(id);
                }
                catch
                {
                    throw new Exception("Couldn't find level using id: " + id);
                }
            }
        }

        public static LevelData GetLevelData(int index, Vector2 position, int tileSize)
        {
            var org = GetLevelOrganisation(index);
            return new LevelData(GetLevelEntities(index, position, GetLevelSize(org, tileSize)), position, GetLevelSize(org, tileSize, tileSize), org, Engine.CurrentMap, GetLevelEnterAction(index));
        }

        public static LevelData GetLevelData(int index, Vector2 position, int tileWidth, int tileHeight)
        {
            var org = GetLevelOrganisation(index);
            return new LevelData(GetLevelEntities(index, position, GetLevelSize(org, tileWidth, tileHeight)), position, GetLevelSize(org, tileWidth, tileHeight), org, Engine.CurrentMap, GetLevelEnterAction(index));
        }

        public static void LoadWorldGrid(LDtkWorld world, int worldDepth)
        {
            int[,] grid = GetWorldGrid(world, worldDepth, out Vector2 gridPos);
            Sprite[,] sp = GetWorldTileSprites(world, worldDepth, gridPos, grid);
            int gridSize = world.Levels.First().GetIntGrid("IntGrid").TileSize;
            Engine.CurrentMap.Instantiate(new Grid(gridPos, gridSize, gridSize, grid, sp));
        }

        private static List<Entity> GetLevelEntities(int index, Vector2 position, Vector2 size)
        {
            Vector2 p = position;

            switch (index)
            {
                case 1:
                    var l = new List<Entity> {
                        new SwingingPoint(new Vector2(Engine.ScreenSize.X / 2, 50), 1000, new Vector2[] { new Vector2(Engine.ScreenSize.X / 2, 50), new Vector2(Engine.ScreenSize.X / 2 - 200, 50) }, new float[]{ 1.5f }, true, Ease.CubeInAndOut),
                        FallDeathTrigger(p, size)
                    };
                    l.AddRange(DefaultLevelTransitions(p, new Level(GetLevelData(2, p + Engine.ScreenSize.OnlyX())), null, null, null));
                    return l;

                case 2:
                    var pulled = new PulledPlatform(new Vector2(60, 200) + p, 200, 40, new Vector2(60, 200) + p + new Vector2(200, 40), 2, Color.Yellow, Ease.CubeInAndOut);
                    var trig = new GrapplingTrigger(new Vector2(60, 200) + p + new Vector2(200, 40), true, pulled.movingTime, pulled.Pull);
                    return new List<Entity> { pulled, trig,
                    new CyclingPlatform(40, 200, new Sprite(Color.YellowGreen), new Vector2[]{ Engine.ScreenSize / 2, Engine.ScreenSize / 2 + new Vector2(-200 ,0) }, new float[] { 1.5f }, Ease.CubeInAndOut),
                    new CyclingPlatform(200, 20, new Sprite(Color.YellowGreen), new Vector2[]{ Engine.ScreenSize / 2 + Engine.ScreenSize.OnlyX() * 0.1f, Engine.ScreenSize / 2 + new Vector2(0 ,-200) }, new float[] { 1.5f }, Ease.CubeInAndOut),
                    new RespawnTrigger(new Vector2(600, 200), new Vector2(200, 300), Vector2.Zero)
                    };

                case 3:
                    return new List<Entity>() { FallDeathTrigger(position, size),
                    new SwingingPoint(new Vector2(200, 20), 1000),
                    new RailedPullBlock(new Vector2[] { new Vector2(50, 10), new Vector2(50, 50), new Vector2(100, 50) }, 1000, 1, 20, 20)
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
