using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public static void DrawEdge(Rectangle rectangle, int lineWidth, Color color)
        {
            spriteBatch.Draw(pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        public static void DebugString()
        {
            Vector2 pos = Vector2.Zero;

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
    }
}
