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
        public Vector2 Direction;
        private Rectangle lightRect;
        /*private Vector2 collidedPoint;
        private Vector2 collidedPoint2;

        private int lightIntensity;
        private int lightIntensity2;*/

        public ProjectorLight(Vector2 position, Vector2 direction, float range) : base(position, 10, 10, new Sprite(Color.White))
        {
            Sprite.Add(Sprite.AllAnimData["ProjectorLight"]);

            lightRect = Sprite.CurrentAnimation.Slices[0].Rect;

            Direction = direction.Normalized();

            /*var r1 = new Raycast(Raycast.RayTypes.MapTiles, Pos + lightRect.Location.ToVector2() + new Vector2(lightRect.Width, 0), VectorHelper.RotateDeg(direction, -range), 300f);
            var r2 = new Raycast(Raycast.RayTypes.MapTiles, Pos + lightRect.Location.ToVector2(), VectorHelper.RotateDeg(direction, range), 300f);
            collidedPoint = r1.EndPoint;
            lightIntensity = (int)(r1.Distance / 1.2f);
            collidedPoint2 = r2.EndPoint;
            lightIntensity2 = (int)(r2.Distance / 1.2f);*/
        }

        public override void Render()
        {
            base.Render();


            Drawing.DrawQuad(
                Pos + lightRect.Location.ToVector2(), new Color(Color.White, 70),
                Pos + lightRect.Location.ToVector2() + new Vector2(lightRect.Width, 0), new Color(Color.White, 70),
                Pos + lightRect.Location.ToVector2() + new Vector2(lightRect.Width, 0) + VectorHelper.RotateDeg(Direction, -45) * 70, new Color(Color.Black, 0),
                Pos + lightRect.Location.ToVector2() + VectorHelper.RotateDeg(Direction, 45) * 70, new Color(Color.Black, 0));
        }
    }
}
