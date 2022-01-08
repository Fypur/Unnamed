using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Components
{
    public abstract class Collider : Component
    {
        public bool Collidable;

        public abstract bool Overlap(Actor actor);

        public abstract bool CollideAt(Vector2 pos);

        public abstract bool CollideWithActorsAt(Vector2 pos);

        public abstract bool CollideAt(Vector2 pos, out Actor entity);

        public abstract bool CollidedWithEntityOfType<T>(Vector2 pos, out T collidedEntity) where T : Actor;

        public abstract bool CollidedWithEntityOfType<T>(Vector2 pos) where T : Actor;

        public abstract bool CollidedWithEntity(Actor e, Vector2 pos) ;

    }
}
