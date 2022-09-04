using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class ProjectorLight : Decoration
    {
        private Vector2 collidedPoint;
        private Vector2 collidedPoint2;

        public ProjectorLight(Vector2 position, Vector2 direction, float range) : base(position, 10, 10, new Sprite(Color.Orange))
        {
            collidedPoint = new Raycast(Raycast.RayTypes.MapTiles, Pos, VectorHelper.RotateDeg(direction, -range), 300f).EndPoint;
            collidedPoint2 = new Raycast(Raycast.RayTypes.MapTiles, Pos, VectorHelper.RotateDeg(direction, range), 300f).EndPoint;
        }

        public override void Render()
        {
            base.Render();
         
            Drawing.DrawQuad(Pos, new Color(Color.White, 120), Pos + Size, new Color(Color.White, 120), collidedPoint, Color.Transparent, collidedPoint2, Color.Transparent);
        }
    }
}
