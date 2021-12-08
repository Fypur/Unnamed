using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Basic_platformer
{
    /// <summary>
    /// Entity that moves and collides with things
    /// </summary>
    public abstract class Actor : Entity
    {
        public Vector2 Pos;
        public int Width;
        public int Height;
        public Vector2 Velocity;
        public float gravityScale;
        public static readonly Vector2 gravityVector = new Vector2(0, 9.81f);

        private float xRemainder;
        private float yRemainder;

        public Actor(Vector2 position, int width, int height, float gravityScale)
        {
            Pos = position;
            Width = width;
            Height = height;
            this.gravityScale = gravityScale;

            Type t = GetType();
            if (!Platformer.EntitiesByType.ContainsKey(t))
                Platformer.EntitiesByType.Add(t, new List<Actor>());
            Platformer.EntitiesByType[t].Add(this);
        }

        protected void MoveX(float amount, Action CallbackOnCollision = null)
        {
            xRemainder += amount;
            int move = (int)Math.Round(xRemainder);

            if(move != 0)
            {
                xRemainder -= move;
                int sign = Math.Sign(amount);

                while (move != 0)
                {
                    if (!CollideAt(Platformer.Solids, Pos + new Vector2(sign, 0)))
                    {
                        Pos.X += sign;
                        move -= sign;
                    }
                    else
                    {
                        CallbackOnCollision?.Invoke();
                        break;
                    }
                }
            }
        }

        public void MoveY(float amount, Action CallbackOnSolidCollision = null)
        {
            yRemainder += amount;
            int move = (int)Math.Round(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(amount);

                while (move != 0)
                {
                    if (!CollideAt(Platformer.Solids, Pos + new Vector2(0, sign)))
                    {
                        Pos.Y += sign;
                        move -= sign;
                    }
                    else
                    {
                        CallbackOnSolidCollision?.Invoke();
                        break;
                    }
                }
            }
        }

        public void MoveTo(Vector2 pos, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null) 
        {
            MoveX(pos.X - Pos.X, CallbackOnCollisionX);
            MoveY(pos.Y - Pos.Y, CallbackOnCollisionY);
        }

        protected bool CollideAt(List<Solid> solids, Vector2 pos)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            foreach (Solid s in solids)
                if (playerRect.Intersects(new Rectangle((int)s.Pos.X, (int)s.Pos.Y, s.Width, s.Height)))
                    return true;

            return false;
        }

        protected bool CollideAt(List<Actor> entities, Vector2 pos)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            foreach (Actor e in entities)
                if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e != this)
                    return true;

            return false;
        }

        protected bool CollideAt(List<Actor> entities, Vector2 pos, out Actor entity)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            foreach (Actor e in entities)
                if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e != this)
                {
                    entity = e;
                    return true;
                }
            entity = null;
            return false;
        }

        protected bool CollidedWithEntityOfType<T>(Vector2 pos, out T collidedEntity) where T : Actor
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            if(Platformer.EntitiesByType.ContainsKey(typeof(T)))
                foreach (Actor e in Platformer.EntitiesByType[typeof(T)])
                    if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e is T castedEntity)
                    {
                        collidedEntity = castedEntity;
                        return true;
                    }

            collidedEntity = null;
            return false;
        }

        protected bool CollidedWithEntityOfType<T>(Vector2 pos) where T : Actor
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            if (Platformer.EntitiesByType.ContainsKey(typeof(T)))
                foreach (Actor e in Platformer.EntitiesByType[typeof(T)])
                    if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e is T castedEntity)
                        return true;

            return false;
        }

        protected bool CollidedWithEntity(Actor e, Vector2 pos)
            => new Rectangle((int)pos.X, (int)pos.Y, Width, Height).Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height));

        public void Gravity()
        {
            Velocity += gravityVector * gravityScale;
        }
    }
}