﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Utility
{
    public static class VectorHelper
    {
        public static float GetAngle(Vector2 vector)
            => (float)Math.Atan2(vector.Y, vector.X);

        //3 Different methods
        /*public static float GetAngle(Vector2 from, Vector2 to)
            => GetAngle(from) - GetAngle(to);
            => (float)Math.Acos(Vector2.Dot(from, to) / (from.Length() * to.Length()) % Math.PI);
            => (float)Math.Atan2(from.X * to.Y - from.Y * to.X, Vector2.Dot(from, to));
        */

        public static float GetAngle(Vector2 from, Vector2 to)
            => (float)Math.Atan2(from.X * to.Y - from.Y * to.X, Vector2.Dot(from, to));

        public static Vector2 Abs(Vector2 vector)
            => new Vector2(Math.Abs(vector.X), Math.Abs(vector.Y));
    }
}
