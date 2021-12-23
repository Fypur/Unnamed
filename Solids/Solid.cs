using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;

namespace Basic_platformer
{
    public abstract class Solid : RenderedEntity
    {
        public Vector2 Pos;
        public int Width;
        public int Height;
        public bool Collidable = true;

        public Solid(Vector2 position, int width, int height)
        {
            this.Pos = position;
            Width = width;
            Height = height;
        }
    }
}