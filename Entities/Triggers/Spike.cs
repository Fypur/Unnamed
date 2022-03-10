using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Spike : DeathTrigger
    {
        public const int size = 8;
        public float Rotation { get => Sprite.Rotation; set => Sprite.Rotation = MathHelper.ToDegrees(value); }
        public static Texture2D texture = DataManager.GetTexture("SpikeTest");

        public Spike(Vector2 position, float rotation)
            : base(position, size, size)
        {
            Sprite = (Sprite)AddComponent(new Sprite(texture, Rect, rotation));
            Sprite.Origin = HalfSize;
            Sprite.Centered = true;
        }
    }
}
