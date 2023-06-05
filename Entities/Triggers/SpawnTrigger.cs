using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class SpawnTrigger : PlayerTrigger
    {
        public IList<Entity> Spawned;
        public SpawnTrigger(Rectangle bounds, IList<Entity> spawned) : base(bounds, null)
        {
            Spawned = spawned;
        }

        public SpawnTrigger(Vector2 position, Vector2 size, IList<Entity> spawned) : base(position, size, null)
        {
            Spawned = spawned;
        }

        public SpawnTrigger(Vector2 position, int width, int height, IList<Entity> spawned) : base(position, width, height, null)
        {
            Spawned = spawned;
        }

        public override void OnTriggerEnter(Player player)
        {
            foreach (Entity e in Spawned)
                Engine.CurrentMap.Instantiate(e);
        }
    }
}
