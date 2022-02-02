using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_platformer
{
    public class DialogueBox : UIElement
    {
        public TextBox CharacterName;
        public TextBox TextBox;
        
        public DialogueBox(string characterName, string text)
            : base(new Vector2(40, 40), 500, 500, new Sprite(Color.White, new Rectangle(40, 40, 500, 500), 0.9f))
        {
            TextBox = new TextBox(text, "Pixel", 0.01f, Pos + new Vector2(12, 30), Width - 20, Height - 20);
            CharacterName = new TextBox(characterName, "Pixel", Pos + new Vector2(12, 10), Width - 20, 20);
            
            AddChild(CharacterName);
            AddChild(TextBox);
            AddComponent(new Sprite(Color.Black, new Rectangle(Pos.ToPoint() + new Point(10, 10), Size.ToPoint() - new Point(20, 20)), 0.8f));
        }
    }
}
