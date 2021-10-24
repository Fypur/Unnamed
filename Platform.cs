using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Platform : Solid
    {
        private Color color = Color.White;

        public Platform(Vector2 position, int width, int height, Color color) : base(position, width, height)
        {
            this.color = color;
        }

        public override void Render()
        {
            Drawing.Draw(new Rectangle((int) Pos.X, (int) Pos.Y, Width, Height), color);
        }
    }
}
