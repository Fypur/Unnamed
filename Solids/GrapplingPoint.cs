using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class GrapplingPoint : CyclingSolid
    {
        private const int width = 7;
        private const int height = 7;

        public GrapplingPoint(Vector2 position) : base(position, width, height, Color.Blue) { }

        public GrapplingPoint(Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easeFunction = null)
            : base(width, height, new Sprite(Color.Blue), positions, timesBetweenPositions, easeFunction) { }

        public override void Render()
        {
            Drawing.Draw(new Rectangle((int)Pos.X, (int)Pos.Y, width, height), Color.Blue);
        }
    }
}
