using Fiourp;
using Microsoft.Xna.Framework;
using System;

namespace Unnamed
{
    public class HurtBox : PlayerTriggerComponent
    {
        public Func<Player, bool> DeathConditions = (player) => true;
        public Action OnDeath = null;
        public bool InstaDeath;
        public HurtBox(Vector2 localPosition, float width, float height) : base(localPosition, width, height)
        { }

        public HurtBox(Vector2 localPosition, float radius) : base(localPosition, radius)
        { }

        public HurtBox(Vector2 localPosition, AABBCollider collider) : base(localPosition, collider) { }

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
