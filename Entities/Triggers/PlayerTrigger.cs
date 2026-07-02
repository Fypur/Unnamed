using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Unnamed
{
    public abstract class PlayerTrigger : Trigger
    {
        public PlayerTrigger(Vector2 position, int width, int height, Sprite sprite)
            : base(position, new AABBCollider(Vector2.Zero, width, height), new List<Type> { typeof(Player) })
        {
            if (sprite != null)
                AddComponent(sprite);
        }

        public PlayerTrigger(Vector2 position, Vector2 size, Sprite sprite)
            : this(position, (int)size.X, (int)size.Y, sprite) { }

        public PlayerTrigger(Rectangle bounds, Sprite sprite)
            : this(bounds.Location.ToVector2(), bounds.Width, bounds.Height, sprite) { }

        public sealed override void OnTriggerEnter(Kinematic entity)
            => OnTriggerEnter(entity as Player);
        public virtual void OnTriggerEnter(Player player) { base.OnTriggerEnter(player); }

        public sealed override void OnTriggerStay(Kinematic entity)
            => OnTriggerStay(entity as Player);
        public virtual void OnTriggerStay(Player player) { }

        public sealed override void OnTriggerExit(Kinematic entity)
            => OnTriggerExit(entity as Player);
        public virtual void OnTriggerExit(Player player) { base.OnTriggerExit(player); }
    }
}
