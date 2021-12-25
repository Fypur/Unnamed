using Basic_platformer.Solids;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;
using Basic_platformer.Triggers;
using Basic_platformer.Utility;

namespace Basic_platformer.Mapping
{
    public class Level
    {
        public int TileWidth = 60;
        public int TileHeight = 60;

        public readonly Vector2 Pos;
        public readonly Vector2 Size;
        public readonly int[,] Organisation;
        public readonly Map ParentMap;
        public readonly int Index;

        private List<Entity> entityData;
        private Action enterAction;

        public Level(int index, Vector2 position, Map parentMap)
        {
            Pos = position;
            Index = index;
            ParentMap = parentMap;
            Organisation = LevelData.GetLevelOrganisation(index);
            Size = LevelData.GetLevelSize(this);
            entityData = LevelData.GetLevelData(this);
            enterAction = LevelData.GetLevelEnterAction(index);
        }

        public void Load()
        {
            Platformer.Cam.SetBoundaries(Pos, Size);
            enterAction();

            for (int y = 0; y < Organisation.GetLength(0); y++)
            {
                for (int x = 0; x < Organisation.GetLength(1); x++)
                {
                    if (Organisation[y, x] == 1)
                        entityData.Add(new SolidTile(Drawing.pointTexture, new Vector2(Pos.X + x * TileWidth, Pos.Y + y * TileHeight), TileWidth, TileHeight));
                }
            }

            foreach (Entity e in entityData)
            {
                ParentMap.Data.Entities.Add(e);

                if(e is RenderedEntity)
                {
                    ParentMap.Data.RenderedEntities.Add((RenderedEntity)e);

                    if (e is Solid)
                    {
                        ParentMap.Data.Solids.Add((Solid)e);

                        if (e is GrapplingTrigger || e is GrapplingPoint)
                            ParentMap.Data.GrapplingSolids.Add((Solid)e);
                    }
                    else if (e is Actor)
                        ParentMap.Data.Actors.Add((Actor)e);
                }
                else if (e is Trigger)
                    ParentMap.Data.Triggers.Add((Trigger)e);
            }
        }

        public void Unload()
        {
            foreach (Entity e in entityData)
            {
                ParentMap.Data.Entities.Remove(e);

                if (e is RenderedEntity)
                {
                    ParentMap.Data.RenderedEntities.Remove((RenderedEntity)e);

                    if (e is Solid)
                    {
                        ParentMap.Data.Solids.Remove((Solid)e);

                        if (e is GrapplingTrigger || e is GrapplingPoint)
                            ParentMap.Data.GrapplingSolids.Remove((Solid)e);
                    }
                    else if (e is Actor)
                        ParentMap.Data.Actors.Remove((Actor)e);
                }
            }
        }

        public override string ToString()
            => $"Level index: {Index} \nPosition: {Pos} \nSize: {Size}";

        public bool Contains(Vector2 point)
            => new Rectangle(Pos.ToPoint(), Size.ToPoint()).Contains(point);

        public Vector2 ToTileCoordinates(Vector2 position)
            => new Vector2((float)Math.Floor(position.X / TileWidth) * TileWidth,
                (float)Math.Floor(position.Y / TileWidth) * TileHeight);

        public Vector2 ToClosestTileCoordinates(Vector2 position)
            => new Vector2((float)Math.Round(position.X / TileWidth) * TileWidth,
                (float)Math.Round(position.Y / TileWidth) * TileHeight);
    }
}
