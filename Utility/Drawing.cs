﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Basic_platformer
{
    public static class Drawing
    {
        private static SpriteBatch spriteBatch;
        public static Texture2D pointTexture;
        public static SpriteFont font;
        
        public static List<string> Debug = new List<string>();
        public static List<string> DebugForever = new List<string>();
        public static List<Tuple<Vector2, Color>> DebugPos = new List<Tuple<Vector2, Color>>();
        public static List<Tuple<Vector2, Color>> DebugPosUpdate = new List<Tuple<Vector2, Color>>();
        public static event Action DebugEvent = delegate { };

        public static void Init(SpriteBatch spriteBatch, SpriteFont font)
        {
            Drawing.spriteBatch = spriteBatch;
            Drawing.font = font;
            pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pointTexture.SetData(new Color[] { Color.White });
        }

        public static void Draw(Texture2D texture, Vector2 position, Color color)
           => spriteBatch.Draw(texture, position, Color.White);

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects = SpriteEffects.None, int layerDepth = 0)
            => spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, spriteEffects, layerDepth);

        public static void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
           => spriteBatch.Draw(texture, destinationRectangle, Color.White);

        public static void Draw(Rectangle rect, Color color)
            => spriteBatch.Draw(pointTexture, rect, color);

        public static void DrawString(string text, Vector2 position, Color color, Vector2 origin)
            => spriteBatch.DrawString(font, text, position, color, 0, origin,
                1, SpriteEffects.None, 1);
        
        public static void DrawString(string text, Vector2 position, Color color, bool centered)
            => spriteBatch.DrawString(font, text, position, color, 0, centered ? font.MeasureString(text) / 2 : Vector2.Zero,
                1, SpriteEffects.None, 1);

        public static void DrawLine(Vector2 begin, Vector2 end, Color color, int thickness = 1)
        {
            float distance = Vector2.Distance(begin, end);
            var angle = (float)Math.Atan2(end.Y - begin.Y, end.X - begin.X);
            var scale = new Vector2(distance, thickness);
            spriteBatch.Draw(pointTexture, begin, null, color, angle, new Vector2(0f, 0.5f), scale, SpriteEffects.None, 0);
        }

        public static void DrawEdge(Rectangle rectangle, int lineWidth, Color color)
        {
            spriteBatch.Draw(pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        public static void DrawPoint(Vector2 pos, int thickness, Color color)
        {
            spriteBatch.Draw(pointTexture, new Rectangle((int)pos.X - (thickness / 2), (int)pos.Y - (thickness / 2), thickness, thickness), color);
        }

        public static void DebugString()
        {
            Vector2 pos = Vector2.Zero;

            if(Debug.Count * font.MeasureString("A").Y + DebugForever.Count * font.MeasureString("A").Y > Platformer.ScreenSize.Y)
                DebugForever.Clear();

            foreach(string s in Debug)
            {
                Drawing.DrawString(s, pos, Color.Brown, Vector2.Zero);
                pos.Y += font.MeasureString(s).Y;
            }

            pos.Y += font.MeasureString(" ").Y;

            foreach (string s in DebugForever)
            {
                Drawing.DrawString(s, pos, Color.Brown, Vector2.Zero);
                pos.Y += font.MeasureString(s).Y;
            }

            Debug.Clear();
        }

        public static void DebugPoint()
        {
            foreach(Tuple<Vector2, Color> pos in DebugPos)
                DrawPoint(pos.Item1, 7, pos.Item2);

            foreach (Tuple<Vector2, Color> pos in DebugPosUpdate)
                DrawPoint(pos.Item1, 7, pos.Item2);

            DebugPosUpdate.Clear();
        }

        public static void DebugEvents()
        {
            DebugEvent();
            DebugEvent = delegate { };
        }
    }
}
