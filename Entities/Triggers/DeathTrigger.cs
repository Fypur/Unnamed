using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class DeathTrigger : PlayerTrigger
    {
        public DeathTrigger(Vector2 position, Vector2 size)
            : base(position, size, null) { }

        public DeathTrigger(Rectangle bounds)
            : base(bounds, null) { }

        public DeathTrigger(Vector2 position, int width, int height)
            : base(position, width, height, null) { }

        public override void OnTriggerEnter(Player player)
        {
            if (!player.Is(Player.States.Dead) && Conditions(player))
                player.Death();
        }

        public override void OnTriggerStay(Player player)
        {
            if (!player.Is(Player.States.Dead) && Conditions(player))
                player.Death();
        }
    }
}
