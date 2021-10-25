using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Basic_platformer
{
    public abstract class Entity
    {
        public Entity(Vector2 position, int width, int height, float gravityScale)
        {
            Pos = position;
            this.Width = width;
            this.Height = height;
            this.gravityScale = gravityScale;
        }

        public Vector2 Pos;
        public int Width;
        public int Height;
        public Vector2 velocity;
        private const float gravity = 9.81f;
        private readonly float gravityScale;

        private float xRemainder;
        private float yRemainder;

        public virtual void Update()
        {

        }
        public virtual void Render() { }

        protected void MoveX(float amount, Action CallbackOnCollision)
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

        public void MoveY(float amount, Action Callback)
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
                        Callback?.Invoke();
                        break;
                    }
                }
            }
        }

        protected bool CollideAt(List<Solid> solids, Vector2 pos)
        {
            Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, Width, Height);
            foreach (Solid s in solids)
                if (new Rectangle((int)s.Pos.X, (int)s.Pos.Y, s.Width, s.Height).Intersects(playerRect))
                    return true;
            return false;
        }

        public void Gravity()
        {
            velocity += new Vector2(0, 9.81f * gravityScale * Platformer.Deltatime);
        }
    }
}