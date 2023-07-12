using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class JetpackActivator : PlayerTrigger
    {
        public JetpackActivator(Rectangle triggerRect) : base(triggerRect, Sprite.None)
        {
        }

        public JetpackActivator(Vector2 position, Vector2 size) : base(position, size, Sprite.None)
        {
        }

        public JetpackActivator(Vector2 position, int width, int height) : base(position, width, height, Sprite.None)
        {
        }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);
            player.CanJetpack = true;
        }
    }
}
