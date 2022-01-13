using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Spike : DeathTrigger
    {
        public const int size = 15;
        public float Rotation;

        public Spike(Vector2 position, float rotation)
            : base(position, size, size)
        {
            Rotation = MathHelper.ToRadians(rotation);
        }

        public override void Render()
        {
            Drawing.Draw(Drawing.pointTexture, Pos, new Rectangle(Pos.ToPoint(), Size.ToPoint()), Color.IndianRed, Rotation, Vector2.Zero,
                Vector2.One, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);
            base.Render();
        }
    }
}
