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
        bool spawned = false;
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
            if (spawned)
                return;

            spawned = true;

            foreach (Entity e in Spawned)
            {
                Engine.CurrentMap.CurrentLevel.DestroyOnUnload(e);
                Engine.CurrentMap.Instantiate(e);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            /*if(spawned)
                foreach (Entity e in Spawned)
                    Engine.CurrentMap.Destroy(e);*/
        }
    }
}
