using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class TextBox : UIElement
    {
        public string Text;
        public string FontID;
        public float TextScale;
        IEnumerator enumerator;
        public enum Style { Normal, Bold, Italic }

        public TextBox(string text, string fontID, Vector2 position, int width, int height)
            : base(position, width, height, new Sprite(Color.White))
        {
            FontID = fontID;
            TextScale = 2;
            Text = SetText(text);
            Sprite.Rect = new Rectangle(Pos.ToPoint(), new Point(Width, Height));
        }

        public TextBox(string text, string fontID, float timePerCharacter, Vector2 position, int width, int height)
            : base(position, width, height, new Sprite(Color.White))
        {
            FontID = fontID;
            TextScale = 2;
            AddComponent(new Coroutine(TextDraw(SetText(text), timePerCharacter)));
            Sprite.Rect = new Rectangle(Pos.ToPoint(), new Point(Width, Height));
        }

        IEnumerator TextDraw(string text, float timePerCharacter)
        {
            foreach(char c in text)
            {
                Text += c;
                yield return new Coroutine.WaitForSeconds(timePerCharacter);
            }
        }

        public string SetText(string text)
        {
            string[] words = text.Split(" ");
            string newText = "";
            float lineSize = 0;
            float spaceSize = DataManager.Fonts[FontID]["Normal"].MeasureString(" ").X * TextScale;
            foreach (string word in words)
            {
                float wordSize = DataManager.Fonts[FontID]["Normal"].MeasureString(word).X * TextScale;
                lineSize += wordSize;

                if (word.Contains('\n'))
                    lineSize = wordSize;

                if (lineSize > Width)
                {
                    newText += "\n";
                    lineSize = wordSize;
                }

                newText += word + " ";
                lineSize += spaceSize;
            }

            Sprite.Scale = DataManager.Fonts[FontID]["Normal"].MeasureString(newText) * TextScale + new Vector2(20, 20);

            return newText;
        }

        public override void Render()
        {
            base.Render();
            if(Text != null)
                Drawing.DrawString(Text, Pos, Color.Black, DataManager.Fonts[FontID]["Normal"], TextScale);
        }
    }
}
