using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnamed
{
    internal class SafeTrigger : PlayerTrigger
    {
        public SafeTrigger(Rectangle bounds) : base(bounds, Sprite.None)
        {
        }

        public SafeTrigger(Vector2 position, Vector2 size) : base(position, size, Sprite.None)
        {
        }

        public SafeTrigger(Vector2 position, int width, int height) : base(position, width, height, Sprite.None)
        {
        }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);
            player.Safe = true;
        }

        public override void OnTriggerExit(Player player)
        {
            base.OnTriggerExit(player);
            player.Safe = false;
        }
    }
}
