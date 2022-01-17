using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class RespawnTrigger : Trigger
    {
        public Vector2 respawnPoint;
        public RespawnTrigger(Vector2 position, Vector2 size, Vector2 respawnPosition)
            : base(position, size, new List<Type> { typeof(Player) }, null)
        {
            this.respawnPoint = respawnPosition;
        }

        public override void OnTriggerEnter(Entity entity)
        {
            Player player = entity as Player;
            player.respawnPoint = respawnPoint;
        }
    }
}
