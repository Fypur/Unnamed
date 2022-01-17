﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class SolidTile : Solid
    {
        public SolidTile(Texture2D texture, Vector2 position, int width, int height) : base(position, width, height,
            new Sprite(texture, new Rectangle(position.ToPoint(), new Point(width, height))))
        { }

        public SolidTile(Color color, Vector2 position, int width, int height) : base(position, width, height, color)
        { }
    }
}