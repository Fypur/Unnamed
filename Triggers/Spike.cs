using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Spike : DeathTrigger
    {
        public const int size = 16;
        public float Rotation;
        public static Texture2D texture = TextureManager.GetTexture("SpikeTest");

        public Spike(Vector2 position, float rotation)
            : base(position, size, size)
        {
            Rotation = MathHelper.ToRadians(rotation);
        }

        public override void Render()
        {
            var origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            Drawing.Draw(texture, Pos + HalfSize, null, Color.White, Rotation, origin,
                Vector2.One * 2, SpriteEffects.None, 1);
            base.Render();
        }
    }
}
