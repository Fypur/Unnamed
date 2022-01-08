using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Components
{
    public class BoxCollider : Collider
    {
        public BoxCollider()
        { }

        public Vector2 Pos { get => parentEntity.Pos; }
        public int Width { get => parentEntity.Width; }
        public int Height { get => parentEntity.Height; }

        public Vector2 Center {
            get => parentEntity.Pos + new Vector2(parentEntity.Width / 2, parentEntity.Height / 2);
        }

        public Vector2 TopRight {
            get => parentEntity.Pos + new Vector2(parentEntity.Width, 0);
        }

        public Vector2 TopLeft {
            get => parentEntity.Pos;
        }

        public Vector2 BottomLeft {
            get => parentEntity.Pos + new Vector2(0, parentEntity.Height);
        }

        public Vector2 BottomRight {
            get => parentEntity.Pos + new Vector2(parentEntity.Width, parentEntity.Height);
        }

        public override bool Overlap(Actor actor)
            => new Rectangle(Pos.ToPoint(), new Point(Width, Height)).Intersects(new Rectangle(actor.Pos.ToPoint(), new Point(actor.Width, actor.Height)));

        public override bool CollideAt(Vector2 pos)
        {
            Rectangle actorRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            foreach (Solid s in Platformer.CurrentMap.Data.Solids)
                if (actorRect.Intersects(new Rectangle((int)s.Pos.X, (int)s.Pos.Y, s.Width, s.Height)) && s.Collidable)
                    return true;

            return false;
        }

        public override bool CollideWithActorsAt(Vector2 pos)
        {
            Rectangle actorRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            foreach (Actor e in Platformer.CurrentMap.Data.Actors)
                if (actorRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e != parentEntity)
                    return true;

            return false;
        }

        public override bool CollideAt(Vector2 pos, out Actor entity)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            foreach (Actor e in Platformer.CurrentMap.Data.Actors)
                if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e != parentEntity)
                {
                    entity = e;
                    return true;
                }
            entity = null;
            return false;
        }

        public override bool CollidedWithEntityOfType<T>(Vector2 pos, out T collidedEntity)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            if (Platformer.CurrentMap.Data.EntitiesByType.ContainsKey(typeof(T)))
                foreach (Actor e in Platformer.CurrentMap.Data.EntitiesByType[typeof(T)])
                    if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e is T castedEntity)
                    {
                        collidedEntity = castedEntity;
                        return true;
                    }

            collidedEntity = null;
            return false;
        }

        public override bool CollidedWithEntityOfType<T>(Vector2 pos)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            if (Platformer.CurrentMap.Data.EntitiesByType.ContainsKey(typeof(T)))
                foreach (Actor e in Platformer.CurrentMap.Data.EntitiesByType[typeof(T)])
                    if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e is T)
                        return true;

            return false;
        }

        public override bool CollidedWithEntity(Actor e, Vector2 pos)
            => new Rectangle((int)pos.X, (int)pos.Y, Width, Height).Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height));
    }
}
