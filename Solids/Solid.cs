using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Basic_platformer
{
    public abstract class Solid : Entity
    {
        public Texture2D Texture;

        public Solid(Vector2 position, int width, int height, Texture2D texture)
            : base(position, width, height)
        {
            Texture = texture;
        }

        public Solid(Vector2 position, int width, int height, Color color)
            : base(position, width, height)
        {
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