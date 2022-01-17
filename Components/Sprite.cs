using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Sprite : Renderer
    {
        public enum DrawingMode { Centered, TopLeft, ScaledTopLeft }

        public float Width { get => Texture.Width; }
        public float Height { get => Texture.Height; }

        public Texture2D Texture;
        public float Rotation;

        public Color Color = Color.White;
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Scale = Vector2.One;
        public SpriteEffects Effect;
        public int LayerDepth;

        public Rectangle Rect;

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public Sprite(Texture2D texture, Vector2 origin)
        {
            Texture = texture;
            Origin = origin;
        }

        public Sprite(Color color)
        {
            Texture = Drawing.pointTexture;
            Color = color;
        }

        public Sprite(Texture2D texture, Rectangle rect)
        {
            Texture = texture;
            Rect = rect;
        }

        public override void Render()
        {
            if (Texture == null)
                return;
            
            if(Rect != null)
                Drawing.Draw(Texture, Rect);
            else
                Drawing.Draw(Texture, ParentEntity.Pos + ParentEntity.HalfSize, null, Color, Rotation, Origin, Scale, Effect, LayerDepth);
        
            
        }
    }
}
