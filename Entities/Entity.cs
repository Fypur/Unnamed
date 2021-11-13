using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Basic_platformer
{
    public abstract class Entity
    {
        public Vector2 Pos;
        public int Width;
        public int Height;
        public Vector2 velocity;
        public float gravityScale;

        public List<Component> Components = new List<Component>();
        private List<Component> componentsToAdd = new List<Component>();
        private List<Component> componentsToRemove = new List<Component>();

        private float xRemainder;
        private float yRemainder;

        public Entity(Vector2 position, int width, int height, float gravityScale)
        {
            Pos = position;
            Width = width;
            Height = height;
            this.gravityScale = gravityScale;

            Type t = GetType();
            if (!Platformer.EntitiesByType.ContainsKey(t))
                Platformer.EntitiesByType.Add(t, new List<Entity>());
            Platformer.EntitiesByType[t].Add(this);
        }

        public virtual void Update()
        {
            foreach (Component c in componentsToAdd)
                Components.Add(c);
            componentsToAdd.Clear();

            foreach (Component c in componentsToRemove)
                Components.Remove(c);
            componentsToRemove.Clear();

            foreach (Component c in Components)
                c.Update();
        }

        public virtual void Render() { }

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

        protected bool CollideAt(List<Solid> solids, Vector2 pos)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            foreach (Solid s in solids)
                if (playerRect.Intersects(new Rectangle((int)s.Pos.X, (int)s.Pos.Y, s.Width, s.Height)))
                    return true;

            return false;
        }

        protected bool CollideAt(List<Entity> entities, Vector2 pos)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            foreach (Entity e in entities)
                if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e!= this)
                    return true;

            return false;
        }

        protected bool CollideAt(List<Entity> entities, Vector2 pos, out Entity entity)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            foreach (Entity e in entities)
                if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e != this)
                {
                    entity = e;
                    return true;
                }
            entity = null;
            return false;
        }

        protected bool CollidedWithEntityOfType<T>(Vector2 pos, out T collidedEntity) where T : Entity
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            if(Platformer.EntitiesByType.ContainsKey(typeof(T)))
                foreach (Entity e in Platformer.EntitiesByType[typeof(T)])
                    if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e is T castedEntity)
                    {
                        collidedEntity = castedEntity;
                        return true;
                    }

            collidedEntity = null;
            return false;
        }

        protected bool CollidedWithEntityOfType<T>(Vector2 pos) where T : Entity
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);

            if (Platformer.EntitiesByType.ContainsKey(typeof(T)))
                foreach (Entity e in Platformer.EntitiesByType[typeof(T)])
                    if (playerRect.Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height)) && e is T castedEntity)
                        return true;

            return false;
        }

        protected bool CollidedWithEntity(Entity e, Vector2 pos)
            => new Rectangle((int)pos.X, (int)pos.Y, Width, Height).Intersects(new Rectangle((int)e.Pos.X, (int)e.Pos.Y, e.Width, e.Height));

        public void Gravity()
        {
            velocity += new Vector2(0, 9.81f * gravityScale * Platformer.Deltatime);
        }

        public void AddComponent(Component component)
        {
            component.parentEntity = this;
            componentsToAdd.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            componentsToRemove.Remove(component);
        }
    }
}