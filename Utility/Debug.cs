﻿using Microsoft.Xna.Framework;
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

        public static void PointUpdate(params Vector2[] log)
        {
            foreach (Vector2 v in log)
                Drawing.DebugPosUpdate.Add(v);
        }

        public static void Line(Vector2 begin, Vector2 end, Color color, int thickness = 1)
            => Drawing.DebugEvent += () => Drawing.DrawLine(begin, end, color, thickness);

        public static void Event(params Action[] actions)
        {
            foreach (Action action in actions)
                Drawing.DebugEvent += action;
        }

        public static void Pause()
            => Platformer.Paused = true;

        public static void Clear()
        {
            Drawing.DebugForever.Clear();
            Drawing.DebugPos.Clear();
        }
    }
}