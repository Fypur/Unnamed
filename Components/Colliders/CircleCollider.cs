using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Components
{
    public class CircleCollider : Collider
    {
        public float Radius;

        public CircleCollider(Vector2 position, float radius)
        {
            Pos = position;
            this.Radius = radius;
        }

        public override bool Collide(BoxCollider other)
            => Collision.RectCircle(other.Bounds, AbsolutePosition, Radius);

        public override bool Collide(CircleCollider other)
            => Vector2.Distance(AbsolutePosition, other.AbsolutePosition) < Radius + other.Radius;

        public override bool Collide(Vector2 point)
            => Vector2.Distance(AbsolutePosition, point) < Radius;

        public override void Render()
        {
            
        }

        public override float Width { get => Radius * 2; set => Radius = value * 0.5f; }
        public override float Height { get => Radius * 2; set => Radius = value * 0.5f; }
        public override float Left { get => Pos.X - Radius; set => Pos.X = value + Radius; }
        public override float Right { get => Pos.X + Radius; set => Pos.X = value - Radius; }
        public override float Top { get => Pos.Y - Radius; set => Pos.X = value + Radius; }
        public override float Bottom { get => Pos.Y + Radius; set => Pos.X = value - Radius; }
    }
}
