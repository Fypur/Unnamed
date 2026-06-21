using Fiourp;
using Microsoft.Xna.Framework;
using System;

namespace Unnamed
{
    public class HurtBox : Component
    {
        public Func<Player, bool> DeathConditions = (player) => true;
        public Action OnDeath = null;
        public bool InstaDeath;
        public HurtBox(Collider collider) { }

        public override void Added()
        {
            base.Added();

            Trigger.Collider.DebugColor = Color.Red;
        }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);

            if (!player.Is(Player.States.Dead) && Conditions(player))
            {
                if (InstaDeath)
                    player.InstaDeath();
                else
                    player.Death();

                OnDeath?.Invoke();
            }
        }

        public override void OnTriggerStay(Player player)
        {
            if (!player.Is(Player.States.Dead) && Conditions(player))
            {
                if (InstaDeath)
                    player.InstaDeath();
                else
                    player.Death();
                OnDeath?.Invoke();
            }
        }

        protected override bool Conditions(Player player)
            => DeathConditions(player);
    }
}
