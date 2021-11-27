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

            Map map = Platformer.Map;
            Vector2 end = begin + Vector2.Normalize(direction) * length;

            Vector2 rayDir = Vector2.Normalize(direction);

            //The hypothenus' size for one Unit (a tile width) on the x and y axis
            Vector2 rayUnitStep = new Vector2((float)Math.Sqrt(map.TileWidth * map.TileWidth + (rayDir.Y * map.TileWidth / rayDir.X) * (rayDir.Y * map.TileWidth / rayDir.X)),
                (float)Math.Sqrt(map.TileHeight * map.TileHeight + (rayDir.X * map.TileHeight / rayDir.Y) * (rayDir.X * map.TileHeight / rayDir.Y)));

            //The tile the begin point is on and the one the end point is on : position truncated to a multiple of the tile's width or height
            Vector2 mapPoint = new Vector2((float)Math.Floor(begin.X / map.TileWidth) * map.TileWidth, (float)Math.Floor(begin.Y / map.TileWidth) * map.TileHeight);

            #endregion

            #region Ray Direction for Each Dimension and Length for non tiled objects

            Vector2 rayStep;
            Vector2 rayLength1D;

            if (rayDir.X < 0)
            {
                rayStep.X = -map.TileWidth;
                rayLength1D.X = (begin.X - mapPoint.X) * rayUnitStep.X / map.TileWidth;
            }
            else
            {
                rayStep.X = map.TileWidth;
                rayLength1D.X = (map.TileWidth + mapPoint.X - begin.X) * rayUnitStep.X / map.TileWidth;

            }
            if (rayDir.Y < 0)
            {
                rayStep.Y = -map.TileHeight;
                rayLength1D.Y = (begin.Y - mapPoint.Y) * rayUnitStep.Y / map.TileWidth;
            }
            else
            {
                rayStep.Y = map.TileHeight;
                rayLength1D.Y = (map.TileWidth + mapPoint.Y - begin.Y) * rayUnitStep.Y / map.TileWidth;
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
                if (mapPoint.X >= 0 && mapPoint.Y >= 0 && mapPoint.X < map.Width && mapPoint.Y < map.Height && travelledDistance < length)
                    if (map.MapOrganisation[(int)mapPoint.Y / map.TileHeight, (int)mapPoint.X / map.TileWidth] > 0)
                        hit = true;
            }

            endPoint = begin + Vector2.Normalize(direction) * length;

            #endregion
        }

    }
}