using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Solids
{
    public class GrapplingPoint : Solid
    {
        private const int width = 7;
        private const int height = 7;
        public GrapplingPoint(Vector2 position) : base(position, width, height) { }

        public override void Render()
        {
            Drawing.Draw(new Rectangle((int)Pos.X, (int)Pos.Y, width, height), Color.AliceBlue);
        }
    }
}
