using Basic_platformer.Mapping;
using Basic_platformer.Solids;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Utility
{
    public class Raycast
    {
        public bool hit;
        public Vector2 endPoint;

        public Raycast(Vector2 begin, Vector2 direction, float length)
            => FastRay(begin, direction, length);


        public Raycast(Vector2 begin, Vector2 end)
            => FastRay(begin, end - begin, Vector2.Distance(begin, end));

        void FastRay(Vector2 begin, Vector2 direction, float length)
        {
            #region Ray Direction, Step Size and Original Pos Tile

            Map map = Platformer.CurrentMap;
            Vector2 end = begin + Vector2.Normalize(direction) * length;

            Vector2 rayDir = Vector2.Normalize(direction);

            //The hypothenus' size for one Unit (a tile width) on the x and y axis
            Vector2 rayUnitStep = new Vector2((float)Math.Sqrt(map.currentLevel.TileWidth * map.currentLevel.TileWidth + (rayDir.Y * map.currentLevel.TileWidth / rayDir.X) * (rayDir.Y * map.currentLevel.TileWidth / rayDir.X)),
                (float)Math.Sqrt(map.currentLevel.TileHeight * map.currentLevel.TileHeight + (rayDir.X * map.currentLevel.TileHeight / rayDir.Y) * (rayDir.X * map.currentLevel.TileHeight / rayDir.Y)));

            //The tile the begin point is on and the one the end point is on : position truncated to a multiple of the tile's width or height
            Vector2 mapPoint = new Vector2((float)Math.Floor(begin.X / map.currentLevel.TileWidth) * map.currentLevel.TileWidth, (float)Math.Floor(begin.Y / map.currentLevel.TileWidth) * map.currentLevel.TileHeight);
            #endregion

            #region Ray Direction for Each Dimension and Length for non tiled objects

            Vector2 rayStep;
            Vector2 rayLength1D;

            if (rayDir.X < 0)
            {
                rayStep.X = -map.currentLevel.TileWidth;
                rayLength1D.X = (begin.X - mapPoint.X) * rayUnitStep.X / map.currentLevel.TileWidth;
            }
            else
            {
                rayStep.X = map.currentLevel.TileWidth;
                rayLength1D.X = (map.currentLevel.TileWidth + mapPoint.X - begin.X) * rayUnitStep.X / map.currentLevel.TileWidth;

            }
            if (rayDir.Y < 0)
            {
                rayStep.Y = -map.currentLevel.TileHeight;
                rayLength1D.Y = (begin.Y - mapPoint.Y) * rayUnitStep.Y / map.currentLevel.TileWidth;
            }
            else
            {
                rayStep.Y = map.currentLevel.TileHeight;
                rayLength1D.Y = (map.currentLevel.TileWidth + mapPoint.Y - begin.Y) * rayUnitStep.Y / map.currentLevel.TileWidth;
            }
            #endregion

            #region Walking the Ray and Checking if it Hit

            float travelledDistance = 0;

            while (!hit && travelledDistance < length)
            {
                //Moving
                if (rayLength1D.X < rayLength1D.Y)
                {
                    mapPoint.X += rayStep.X;
                    travelledDistance = rayLength1D.X;
                    rayLength1D.X += rayUnitStep.X;
                }
                else
                {
                    mapPoint.Y += rayStep.Y;
                    travelledDistance = rayLength1D.Y;
                    rayLength1D.Y += rayUnitStep.Y;
                }

                //Checking
                if (mapPoint.X >= 0 && mapPoint.Y >= 0 && travelledDistance < length)
                    if (map.currentLevel.LevelOrganisation[(int)mapPoint.Y / map.currentLevel.TileHeight, (int)mapPoint.X / map.currentLevel.TileWidth] > 0)
                        hit = true;
            }
            
            endPoint = begin + Vector2.Normalize(direction) * travelledDistance;

            #endregion
        }

    }
}