using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class DeathTrigger : Trigger
    {
        public DeathTrigger(Vector2 position, Vector2 size)
            : base(position, size, new List<Type> { typeof(Player) }) { }

        public DeathTrigger(Vector2 position, int width, int height)
            : base(position, width, height, new List<Type> { typeof(Player) }) { }

        public override void OnTriggerEnter(Entity entity)
        {
            (entity as Player).Death();
        }
    }
}
