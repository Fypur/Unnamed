using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public abstract class PlayerTrigger : Trigger
    {
        public Func<Player, bool> Conditions = (player) => true;

        public PlayerTrigger(Vector2 position, Vector2 size, Sprite sprite)
            : base(position, (int)size.X, (int)size.Y, new List<Type> { typeof(Player) }, sprite) { }

        public PlayerTrigger(Vector2 position, int width, int height, Sprite sprite)
            : base(position, new Vector2(width, height), new List<Type> { typeof(Player) }, sprite) { }

        public PlayerTrigger(Rectangle bounds, Sprite sprite)
            : base(bounds.Location.ToVector2(), bounds.Size.ToVector2(), new List<Type> { typeof(Player) }, sprite) { }

        public sealed override void OnTriggerEnter(Entity entity)
            => OnTriggerEnter(entity as Player);
        public virtual void OnTriggerEnter(Player player) { }

        public sealed override void OnTriggerStay(Entity entity)
            => OnTriggerStay(entity as Player);
        public virtual void OnTriggerStay(Player player) { }

        public sealed override void OnTriggerExit(Entity entity)
            => OnTriggerExit(entity as Player);
        public virtual void OnTriggerExit(Player player) { }
    }
}
