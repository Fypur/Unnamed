using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Solids
{
    public class CyclingPlatform : CyclingSolid
    {
        public Color Color;

        public CyclingPlatform(int width, int height, Color color, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(width, height, positions, timesBetweenPositions, easingfunction) { Color = color; }

        public override void Render()
        {
            Drawing.Draw(new Rectangle(Pos.ToPoint(), new Point(Width, Height)), Color);
        }
    }
}
