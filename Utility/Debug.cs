using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Utility
{
    public static class Debug
    {
        public static bool DebugMode;

        public static void LogUpdate(params object[] log)
        {
            foreach (object l in log)
                Drawing.Debug.Add(l.ToString());
        }

        public static void Log(params object[] log)
        {
            foreach (object l in log)
                Drawing.DebugForever.Add(l.ToString());
        }

        public static void Point(params Vector2[] log)
        {
            foreach (Vector2 v in log)
                Drawing.DebugPos.Add(v);
        }

        public static void Clear()
        {
            Drawing.DebugForever.Clear();
            Drawing.DebugPos.Clear();
        }
    }
}