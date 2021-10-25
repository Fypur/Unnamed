using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public static class Debug
    {
        public static void Log(object log)
            => Drawing.Debug.Add(log.ToString());
    }
}