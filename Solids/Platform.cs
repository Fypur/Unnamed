﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Platform : Solid
    {
        public Platform(Vector2 position, int width, int height, Color color) : base(position, width, height, color)
        { }
    }
}
