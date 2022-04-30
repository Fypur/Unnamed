using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class HurtBox : PlayerTriggerComponent
    {
        public Func<Player, bool> DeathConditions = (player) => true;
        public HurtBox(Vector2 localPosition, float width, float height) : base(localPosition, width, height)
        { }

        public HurtBox(Vector2 localPosition, float radius) : base(localPosition, radius)
        { }

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

        protected override bool Conditions(Player player)
            => DeathConditions(player);
    }
}
