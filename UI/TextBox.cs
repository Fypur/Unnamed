﻿using Microsoft.Xna.Framework;
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
        public enum Style { Normal, Bold, Italic }

        public TextBox(string text, string fontID, Vector2 position, int width, int height, int fontSize = 3)
            : base(position, width, height, null)
        {
            FontID = fontID;
            TextScale = fontSize;
            Text = GenerateText(text);
        }

        public TextBox(string text, string fontID, float timePerCharacter, Vector2 position, int width, int height, int fontSize = 3)
            : base(position, width, height, null)
        {
            FontID = fontID;
            TextScale = fontSize;
            ProgressiveDraw(GenerateText(text), timePerCharacter);
        }

        public void ProgressiveDraw(string text, float timePerCharacter, bool pregeneratedString = false)
            => AddComponent(new Coroutine(TextDraw(pregeneratedString ? text : GenerateText(text), timePerCharacter))); 

        public void StopProgressiveDraw()
        {
            if (GetComponent(out Coroutine couroutine))
                RemoveComponent(couroutine);
        }

        private IEnumerator TextDraw(string text, float timePerCharacter)
        {
            foreach(char c in text)
            {
                Text += c;
                yield return new Coroutine.WaitForSeconds(timePerCharacter);
            }
        }

        public void SetText(string text)
            => Text = GenerateText(text);

        public string GenerateText(string text)
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

            return newText.TrimEnd();
        }

        public void Reset()
        {
            StopProgressiveDraw();
            Text = "";
        }

        public override void Render()
        {
            base.Render();
            if(Text != null)
                Drawing.DrawString(Text, Pos, Color.White, DataManager.Fonts[FontID]["Normal"], TextScale);
        }
    }
}
