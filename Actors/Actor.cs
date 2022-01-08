using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Basic_platformer.Entities;
using Basic_platformer.Components;

namespace Basic_platformer
{
    /// <summary>
    /// Entity that moves and collides with things
    /// </summary>
    public abstract class Actor : Entity
    {
        public Vector2 Velocity;

        protected Vector2 HalfSize { get => new Vector2(Width / 2, Height / 2); }

        public float gravityScale;
        public static readonly Vector2 gravityVector = new Vector2(0, 9.81f);

        public Collider Collision;

        private float xRemainder;
        private float yRemainder;


        public virtual bool IsRiding(Solid solid)
            => new Rectangle(Pos.ToPoint(), new Point(Width, Height + 1)).Intersects(new Rectangle(solid.Pos.ToPoint(), new Point(solid.Width, solid.Height)));

        public virtual void Squish() 
            => Platformer.CurrentMap.Destroy(this);


        public Actor(Vector2 position, int width, int height, float gravityScale) 
            : base(position, width, height)
        {
            this.gravityScale = gravityScale;

            Collision = new BoxCollider();
            AddComponent(Collision);

            #region Entities By Type
            Type t = GetType();
            if (!Platformer.CurrentMap.Data.EntitiesByType.ContainsKey(t))
                Platformer.CurrentMap.Data.EntitiesByType.Add(t, new List<Actor>());
            Platformer.CurrentMap.Data.EntitiesByType[t].Add(this); 
            #endregion
        }

        public void MoveX(float amount, Action CallbackOnCollision = null)
        {
            xRemainder += amount;
            int move = (int)Math.Round(xRemainder);

            if(move != 0)
            {
                xRemainder -= move;
                int sign = Math.Sign(amount);

                while (move != 0)
                {
                    if (!Collision.CollideAt(Pos + new Vector2(sign, 0)))
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
                    if (!Collision.CollideAt(Pos + new Vector2(0, sign)))
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

        public void Gravity()
        {
            Velocity += gravityVector * gravityScale;
        }
    }
}