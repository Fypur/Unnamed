using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class PlayerTriggerComponent : TriggerComponent
    {
        protected virtual bool Conditions(Player player) => true;

        public PlayerTriggerComponent(Vector2 localPosition, float width, float height) 
            : base(localPosition, width, height, new List<Type> { typeof(Player) })
        { }

        public PlayerTriggerComponent(Vector2 localPosition, float radius)
            : base(localPosition, radius, new List<Type> { typeof(Player) })
        { }

        public PlayerTriggerComponent(Vector2 localPosition, Collider collider)
            : base(localPosition, collider, new List<Type> { typeof(Player) })
        { }

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
