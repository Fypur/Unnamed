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

        /*protected void MoveX(float amount)
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
                    actor.Pos.X = movingRect.X + (Math.Sign(move) == 1 ? movingRect.Width : -actor.Width);
                }
            }

            //Move Solid
            Pos.X += move;
        }
        */
        /*protected void MoveY(float amount)
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
                    actor.Pos.Y = movingRect.Y + (Math.Sign(move) == 1 ? movingRect.Height : -actor.Height);
                }
            }

            //Move Solid
            Pos.Y += move;
        }*/

        public void Move(float x, float y)
        {
            xRemainder += x;
            yRemainder += y;
            int moveX = (int)Math.Round(xRemainder);
            int moveY = (int)Math.Round(yRemainder);

            if (moveX == 0 && moveY == 0) return;

            List<Actor> ridingActors = GetAllRidingActors();
            Collidable = false;

            if(moveX != 0)
            {
                xRemainder -= moveX;
                Pos.X += moveX;

                if(moveX > 0)
                {
                    foreach (Actor actor in Platformer.CurrentMap.Data.Actors)
                    {
                        if (OverlapCheck(actor))
                        {
                            actor.MoveX(Pos.X + Width - actor.Pos.X, actor.Squish);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveX(moveX, null);
                        }
                    }
                }
                else
                {
                    foreach (Actor actor in Platformer.CurrentMap.Data.Actors)
                    {
                        if (OverlapCheck(actor))
                        {
                            actor.MoveX(Pos.X - actor.Pos.X - actor.Width, actor.Squish);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveX(moveX, null);
                        }
                    }
                }
            }
            
            if(moveY != 0)
            {
                yRemainder -= moveY;
                Pos.Y += moveY;

                if (moveY > 0)
                {
                    foreach (Actor actor in Platformer.CurrentMap.Data.Actors)
                    {
                        if (OverlapCheck(actor))
                        {
                            actor.MoveY(Pos.Y + Height - actor.Pos.Y, actor.Squish);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveY(moveY, null);
                        }
                    }
                }
                else
                {
                    foreach (Actor actor in Platformer.CurrentMap.Data.Actors)
                    {
                        if (OverlapCheck(actor))
                        {
                            actor.MoveY(Pos.Y - actor.Pos.Y - actor.Height, actor.Squish);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveY(moveY, null);
                        }
                    }
                }
            }

            Collidable = true;
        }

        protected void MoveTo(Vector2 pos)
        {
            Move(pos.X - Pos.X, pos.Y - Pos.Y);
        }

        void Gravity()
            => Velocity += gravityVector * gravityScale;

        private List<Actor> GetAllRidingActors()
        {
            List<Actor> ridingActors = new List<Actor>();

            foreach(Actor a in Platformer.CurrentMap.Data.Actors)
            {
                if (a.IsRiding(this) == true)
                    ridingActors.Add(a);
            }

            return ridingActors;
        }

        private bool OverlapCheck(Actor actor)
            => new Rectangle(Pos.ToPoint(), new Point(Width, Height)).Intersects(new Rectangle(actor.Pos.ToPoint(), new Point(actor.Width, actor.Height)));
    }
}
