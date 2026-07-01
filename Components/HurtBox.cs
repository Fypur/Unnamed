using Fiourp;
using Microsoft.Xna.Framework;
using System;

namespace Unnamed
{
    public class HurtBox : Component
    {
        public Func<Player, bool> DeathConditions = (player) => true;
        public Collider Collider;
        public Action OnDeath = null;
        public bool InstaDeath;
        public HurtBox(Collider collider)
        {
            Collider = collider;
            collider.DebugColor = Color.Red;
        }

        public override void Update()
        {
            base.Update();

            if (Collider.Collide(Platformer.Player.Collider) && !Platformer.Player.Is(Player.States.Dead) && !DeathConditions(Platformer.Player))
            {
                if (InstaDeath)
                    Platformer.Player.InstaDeath();
                else
                    Platformer.Player.Death();

                OnDeath?.Invoke();
            }
        }
    }
}
