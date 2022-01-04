using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace Basic_platformer
{
    public abstract class Solid : RenderedEntity
    {
        public Vector2 Pos;
        public int Width;
        public int Height;

        public Texture2D Texture;

        public bool Collidable = true;

        public Solid(Vector2 position, int width, int height, Texture2D texture)
        {
            Pos = position;
            Width = width;
            Height = height;
            Texture = texture;
        }

        public Solid(Vector2 position, int width, int height, Color color)
        {
            Pos = position;
            Width = width;
            Height = height;
            Texture = new Texture2D(Platformer.graphics.GraphicsDevice, 1, 1);
            Texture.SetData(new Color[] { color });
        }

        public override void Render()
        {
            base.Render();

            Drawing.Draw(Texture, new Rectangle(Pos.ToPoint(), new Point(Width, Height)), Color.White);
        }
    }
}