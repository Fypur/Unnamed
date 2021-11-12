using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
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

        public static void Clear()
            => Drawing.DebugForever.Clear();
    }
}