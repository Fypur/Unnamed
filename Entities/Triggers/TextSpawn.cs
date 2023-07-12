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
        public string Text;
        public WritingTextBox TextBox;

        public TextSpawn(Vector2 position, Vector2 size, Vector2 textPos, Color color, string text) : base(position, size, Sprite.None)
        {
            Text = text;
            Collider.DebugColor = Color.LightGreen;
            TextBox = new WritingTextBox("", "Recursive", textPos, int.MaxValue, int.MaxValue, 0.3f, color, false, Fiourp.TextBox.Alignement.TopLeft, 0.01f);
        }

        public override void Awake()
        {
            base.Awake();
            AddChild(TextBox); //Need to put this after awake so that position isn't transformed when screen scale
        }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);

            TextBox.StopAllCoroutines();
            TextBox.ProgressiveDraw(Text[TextBox.Text.Length..], 0.01f, true);
        }

        public override void OnTriggerExit(Player player)
        {
            base.OnTriggerExit(player);

            TextBox.StopAllCoroutines();
            TextBox.ProgressiveRemove(0.01f);
        }
    }
}
