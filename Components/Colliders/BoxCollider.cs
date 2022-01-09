﻿using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Components
{
    public class BoxCollider : Collider
    {
        private float width;
        private float height;

        public BoxCollider(Vector2 position, int width, int height)
        {
            Pos = position;
            this.width = width;
            this.height = height;
        }
        
        public override bool Collide(BoxCollider other)
            => Bounds.Intersects(other.Bounds);

        public override bool Collide(CircleCollider other)
            => Collision.RectCircle(Bounds, other.Pos, other.Radius);

        public override bool Collide(Vector2 point)
            => Bounds.Contains(point);

        public override float Width { get => width; set => width = value; }
        public override float Height { get => height; set => height = value; }
        public override float Left { get => Pos.X; set => Pos.X = value; }
        public override float Right { get => Pos.X + width; set => Pos.X = value - width; }
        public override float Top { get => Pos.Y; set => Pos.Y = value; }
        public override float Bottom { get => Pos.Y + height; set => Pos.Y = value - height; }

    }
}
