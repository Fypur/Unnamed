﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_platformer
{
    public class DialogueBox : UIElement
    {
        public TextBox TextBox;
        public string[] Dialogue;

        private int currentTextIndex = 0;
        private string currentGeneratedString;
        
        public DialogueBox(string[] dialogue)
            : base(new Vector2(140, 20), 1000, 300, new Sprite(Color.White, new Rectangle(140, 20, 1000, 300), 0.9f))
        {
            Dialogue = dialogue;
            TextBox = new TextBox(dialogue[0], "Pixel", 0.01f, Pos + new Vector2(20, 20), Width - 30, Height - 20);
            currentGeneratedString = TextBox.GenerateText(dialogue[0]);

            AddChild(TextBox);
            AddComponent(new Sprite(Color.Black, new Rectangle(Pos.ToPoint() + new Point(10, 10), Size.ToPoint() - new Point(20, 20)), 0.8f));
        }

        public override void Update()
        {
            base.Update();
            if(Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                if (TextBox.Text == currentGeneratedString && currentTextIndex + 1 < Dialogue.Length)
                {
                    currentTextIndex++;
                    currentGeneratedString = TextBox.GenerateText(Dialogue[currentTextIndex]);
                    TextBox.Reset();
                    TextBox.ProgressiveDraw(currentGeneratedString, 0.01f, true);
                }
                else
                {
                    TextBox.StopProgressiveDraw();
                    TextBox.SetText(currentGeneratedString);
                }
            }
        }
    }
}
