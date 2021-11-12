using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Static_Classes
{
    public static class Ease
    {
        public static float QuintIn(float t)
            => (float)Math.Pow(t, 5);

        public static float QuintOut(float t)
            => 1 - (float) Math.Pow(1 - t, 5);

        public static float Reverse(float t)
            => -t + 1;
    }
}