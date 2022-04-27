using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class RespawnTrigger : PlayerTrigger
    {
        public Vector2 RespawnPoint;
        public RespawnTrigger(Vector2 position, Vector2 size, Vector2 respawnPosition)
            : base(position, size, null)
        {
            RespawnPoint = respawnPosition;
        }

        public override void OnTriggerEnter(Entity entity)
        {
            ((Player)entity).respawnPoint = RespawnPoint;
        }
    }
}
