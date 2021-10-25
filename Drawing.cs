using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Basic_platformer
{
    public static class Drawing
    {
        private static SpriteBatch spriteBatch;
        private static Texture2D whiteTexture;
        public static SpriteFont font;
        public static List<string> Debug = new List<string>();

        public static void Init(SpriteBatch spriteBatch, SpriteFont font)
        {
            Drawing.spriteBatch = spriteBatch;
            Drawing.font = font;
            whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            whiteTexture.SetData(new Color[] { Color.White });
        }

        public static void Draw(Texture2D texture, Vector2 position, Color color)
           => spriteBatch.Draw(texture, position, Color.White);

        public static void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
           => spriteBatch.Draw(texture, destinationRectangle, Color.White);

        public static void Draw(Rectangle rect, Color color)
            => spriteBatch.Draw(whiteTexture, rect, color);

        public static void DrawString(string text, Vector2 position, Color color, Vector2 origin)
            => spriteBatch.DrawString(font, text, position, color, 0, origin,
                1, SpriteEffects.None, 1);

        public static void DebugString()
        {
            Vector2 pos = Vector2.Zero;
            foreach(string s in Debug)
            {
                Drawing.DrawString(s, pos, Color.Aqua, Vector2.Zero);
                pos.Y += font.MeasureString(s).Y;
            }

            Debug.Clear();
        }
    }
}
