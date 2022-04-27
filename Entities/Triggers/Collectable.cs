using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public abstract class Collectable : PlayerTrigger
    {
        private bool collected;

        public Collectable(Vector2 position, int width, int height, Sprite sprite)
            : base(position, width, height, sprite) { }

        public override void OnTriggerEnter(Entity entity)
        {
            if (!collected)
            {
                collected = true;
                AddComponent(new Coroutine(OnCollected((Player)entity)));
            }
        }

        public abstract IEnumerator OnCollected(Player player);
    }
}
