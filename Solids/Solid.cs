using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;
using Microsoft.Xna.Framework.Graphics;
using Basic_platformer.Components;

namespace Basic_platformer
{
    public abstract class Solid : Entity
    {
        public Texture2D Texture;

        public Collider Collision;

        public bool Collidable = true;

        public Solid(Vector2 position, int width, int height, Texture2D texture)
            : base(position, width, height)
        {
            Texture = texture;
            Collision = new BoxCollider();
            AddComponent(Collision);
        }

        public Solid(Vector2 position, int width, int height, Color color)
            : base(position, width, height)
        {
            Collision = new BoxCollider();
            AddComponent(Collision);

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