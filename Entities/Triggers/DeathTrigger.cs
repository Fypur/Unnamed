using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class DeathTrigger : Trigger
    {
        public Func<Player, bool> Conditions = (player) => true;

        public DeathTrigger(Vector2 position, Vector2 size)
            : base(position, size, new List<Type> { typeof(Player) }, null) { }

        public DeathTrigger(Vector2 position, int width, int height)
            : base(position, width, height, new List<Type> { typeof(Player) }, null) { }

        public override void OnTriggerEnter(Entity entity)
        {
            Player p = entity as Player;
            if(Conditions(p))
                p.Death();
        }
    }
}
