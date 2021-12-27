using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Utility
{
    public static class VectorHelper
    {
        public static float GetAngle(Vector2 vector)
            => (float)Math.Atan2(vector.Y, vector.X);

        public static float GetAngle(Vector2 from, Vector2 to)
            => GetAngle(from) - GetAngle(to);
    }
}
