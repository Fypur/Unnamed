using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Solids
{
    public abstract class MovingSolid : Solid
    {
        public Vector2 Velocity;
        protected List<Actor> ridingActors;

        private float xRemainder;
        private float yRemainder;

        protected float gravityScale = 0;
        protected static readonly Vector2 gravityVector = new Vector2(0, 9.81f);

        public MovingSolid(Vector2 position, int width, int height) : base (position, width, height) { }

        protected void MoveX(float amount, Action CallbackOnCollisionX = null)
        {
            int move = (int)amount;
            xRemainder += amount - move;

            if(xRemainder >= 1)
            {
                move += (int)xRemainder;
                xRemainder -= (int)xRemainder;
            }

            if (move == 0)
                return;

            //Check all entities between this solid and it's final Pos
            Rectangle movingRect;
            if (Math.Sign(move) == 1)
                movingRect = new Rectangle((int)Pos.X, (int)Pos.Y, Width + move, Height);
            else
                movingRect = new Rectangle((int)Pos.X + move, (int)Pos.Y, Width - move, Height);

            foreach (Actor actor in Platformer.CurrentMap.Data.Actors)
            {
                //Take those entities and move them
                if (new Rectangle((int)actor.Pos.X, (int)actor.Pos.Y, actor.Width, actor.Height).Intersects(movingRect))
                {
                    CallbackOnCollisionX?.Invoke();
                    actor.Pos.X = movingRect.X + (Math.Sign(move) == 1 ? movingRect.Width : -actor.Width);
                }
            }

            //Move Solid
            Pos.X += move;
        }

        protected void MoveY(float amount, Action CallbackOnCollisionY = null)
        {
            int move = (int)amount;
            yRemainder += amount - move;

            if (yRemainder >= 1)
            {
                move += (int)yRemainder;
                yRemainder -= (int)yRemainder;
            }

            if (move == 0)
                return;

            //Check all entities between this solid and it's final Pos
            Rectangle movingRect;
            if (Math.Sign(move) == 1)
                movingRect = new Rectangle((int)Pos.X, (int)Pos.Y, Width, move + Height);
            else
                movingRect = new Rectangle((int)Pos.X, (int)Pos.Y + move, Width, -move + Height);

            foreach (Actor actor in Platformer.CurrentMap.Data.Actors)
            {
                //Take those entities and move them
                if (new Rectangle((int)actor.Pos.X, (int)actor.Pos.Y, actor.Width, actor.Height).Intersects(movingRect))
                {
                    CallbackOnCollisionY?.Invoke();
                    actor.Pos.Y = movingRect.Y + (Math.Sign(move) == 1 ? movingRect.Height : -actor.Height);
                }
            }

            //Move Solid
            Pos.Y += move;
        }

        protected void MoveTo(Vector2 pos, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null)
        {
            MoveX(pos.X - Pos.X, CallbackOnCollisionX);
            MoveY(pos.Y - Pos.Y, CallbackOnCollisionY);
        }

        void Gravity()
        {
            Velocity += gravityVector * gravityScale;
        }
    }
}
