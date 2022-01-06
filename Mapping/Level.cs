using Basic_platformer.Solids;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;
using Basic_platformer.Triggers;
using Basic_platformer.Utility;
using Microsoft.Xna.Framework.Graphics;

namespace Basic_platformer.Mapping
{
    public class Level
    {
        public int TileWidth = 24;
        public int TileHeight = 24;

        public readonly Vector2 Pos;

        public readonly Vector2 Size;
        public readonly int[,] Organisation;
        public readonly Vector2[] Corners;

        public readonly Map ParentMap;
        public readonly int Index;

        private List<Entity> entityData;
        private Action enterAction = null;

        public Level(int index, Vector2 position, Map parentMap)
        {
            Pos = position;
            Index = index;
            ParentMap = parentMap;

            Organisation = LevelData.GetLevelOrganisation(index);
            Size = LevelData.GetLevelSize(this);
            Corners = GetLevelCorners();

            entityData = LevelData.GetLevelData(this);
            enterAction = LevelData.GetLevelEnterAction(index);
            GetLevelCorners();
        }

        public void Load()
        {
            Platformer.Cam.SetBoundaries(Pos, Size);
            enterAction?.Invoke();

            for (int y = 0; y < Organisation.GetLength(0); y++)
            {
                for (int x = 0; x < Organisation.GetLength(1); x++)
                {
                    if (Organisation[y, x] != 0)
                    {
                        entityData.Add(new SolidTile(GetTileTexture(x, y), new Vector2(Pos.X + x * TileWidth, Pos.Y + y * TileHeight), TileWidth, TileHeight));
                    }
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

        public Vector2[] GetLevelCorners()
        {
            List<Vector2> points = new List<Vector2>();
            for (int x = 0; x < Organisation.GetLength(1); x++)
            {
                for (int y = 0; y < Organisation.GetLength(0); y++)
                {
                    if(Organisation[y, x] != 0)
                    {
                        if (GetOrganisation(x - 1, y) == 0 && GetOrganisation(x, y - 1) == 0 && GetOrganisation(x - 1, y - 1) == 0)
                            points.Add(new Vector2(x * TileWidth, y * TileHeight));

                        if (GetOrganisation(x + 1, y) == 0 && GetOrganisation(x, y - 1) == 0 && GetOrganisation(x + 1, y - 1) == 0)
                            points.Add(new Vector2((x + 1) * TileWidth, y * TileHeight));

                        if (GetOrganisation(x - 1, y) == 0 && GetOrganisation(x, y + 1) == 0 && GetOrganisation(x - 1, y + 1) == 0)
                            points.Add(new Vector2(x * TileWidth, (y + 1) * TileHeight));

                        if (GetOrganisation(x + 1, y) == 0 && GetOrganisation(x, y + 1) == 0 && GetOrganisation(x + 1, y + 1) == 0)
                            points.Add(new Vector2((x + 1) * TileWidth, (y + 1) * TileHeight));
                    }
                }
            }

            return points.ToArray();
        }

        public int GetOrganisation(int x, int y, int returnIfEmpty = 0)
        {
            if(x >= 0 && x < Organisation.GetLength(1) && y >= 0 && y < Organisation.GetLength(0))
                return Organisation[y, x];
            else
                return returnIfEmpty;
        }

        private Texture2D GetTileTexture(int x, int y) 
        {
            int tileValue = Organisation[y, x];
            Dictionary<string, Texture2D> tileSet = TileData.TileSets[tileValue];

            bool rightBlock = GetOrganisation(x + 1, y, tileValue) != 0;
            bool leftBlock = GetOrganisation(x - 1, y, tileValue) != 0;
            bool topBlock = GetOrganisation(x, y - 1, tileValue) != 0;
            bool bottomBlock = GetOrganisation(x, y + 1, tileValue) != 0;

            bool topRightBlock = GetOrganisation(x + 1, y - 1, tileValue) != 0;
            bool topLeftBlock = GetOrganisation(x - 1, y - 1, tileValue) != 0;
            bool bottomRightBlock = GetOrganisation(x + 1, y + 1, tileValue) != 0;
            bool bottomLeftBlock = GetOrganisation(x - 1, y - 1, tileValue) != 0;

            if (!leftBlock && !rightBlock && !topBlock && !bottomBlock)
                return tileSet["complete"];

            if (!topBlock && rightBlock && leftBlock && bottomBlock)
                return tileSet["top"];

            if (!bottomBlock && rightBlock && leftBlock && topBlock)
                return tileSet["bottom"];

            if (topBlock && bottomBlock && !rightBlock && leftBlock)
                return tileSet["right"];

            if (topBlock && bottomBlock && rightBlock && !leftBlock)
                return tileSet["left"];

            if (!topBlock && rightBlock && !leftBlock && !bottomBlock)
                return tileSet["leftFullCorner"];

            if (!topBlock && !rightBlock && leftBlock && !bottomBlock)
                return tileSet["rightFullCorner"];

            if (!topBlock && !rightBlock && !leftBlock && bottomBlock)
                return tileSet["upFullCorner"];

            if (topBlock && !rightBlock && !leftBlock && !bottomBlock)
                return tileSet["downFullCorner"];

            if (!topBlock && leftBlock && rightBlock && !bottomBlock)
                return tileSet["horizontalPillar"];

            if (topBlock && !leftBlock && !rightBlock && bottomBlock)
                return tileSet["verticalPillar"];

            if (!topBlock && !rightBlock)
                return tileSet["topRightCorner"];

            if (!topBlock && !leftBlock)
                return tileSet["topLeftCorner"];

            if (!bottomBlock && !rightBlock)
                return tileSet["bottomRightCorner"];

            if (!bottomBlock && !leftBlock)
                return tileSet["bottomLeftCorner"];

            if (topBlock && rightBlock)
                return tileSet["topRightPoint"];

            if (topBlock && leftBlock)
                return tileSet["topLeftPoint"];

            if (bottomBlock && rightBlock)
                return tileSet["bottomRightPoint"];

            if (bottomBlock && leftBlock)
                return tileSet["bottomLeftPoint"];

            return tileSet["inside"];
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
