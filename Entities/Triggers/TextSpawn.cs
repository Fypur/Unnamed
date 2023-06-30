using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class TextSpawn : PlayerTrigger
    {
        Vector2 initPos;
        public string Text;
        public WritingTextBox TextBox;

        public TextSpawn(Vector2 position, Vector2 size, Vector2 textPos, Color color, string text) : base(position, size, Sprite.None)
        {
            Text = text;
            initPos = textPos;
            Collider.DebugColor = Color.LightGreen;
            TextBox = new WritingTextBox("", "Recursive", textPos, int.MaxValue, int.MaxValue, 0.3f, color, false, Fiourp.TextBox.Alignement.TopLeft, 0.01f);
            AddChild(TextBox);
        }

        public override void OnTriggerEnter(Player player)
        {
            TextBox.StopAllCoroutines();
            TextBox.ProgressiveDraw(Text[TextBox.Text.Length..], 0.01f, true);
        }

        public override void OnTriggerExit(Player player)
        {
            TextBox.StopAllCoroutines();
            TextBox.ProgressiveRemove(0.01f);
        }
    }
}
