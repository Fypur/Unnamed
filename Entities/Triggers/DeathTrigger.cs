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

        public override void OnTriggerEnter(Entity entity)
        {
            Player p = entity as Player;
            if(Conditions(p))
                p.Death();
        }

        public override void OnTriggerStay(Entity entity)
        {
            Player p = entity as Player;
            if (Conditions(p))
                p.Death();
        }
    }
}
