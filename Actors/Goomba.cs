using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Goomba : Actor
    {
        private const float constGravityScale = 2f;
        private const float speed = 100f;

        private bool onGround;

        public Goomba(Vector2 position, int width, int height) : base(position, width, height, constGravityScale, new Sprite(Color.Blue))
        {
            Velocity.X = -speed;
        }

        public override void Update()
        {
            onGround = Collider.CollideAt(Pos + new Vector2(0, 1));

            if (Collider.CollideAt(Pos + new Vector2(1, 0)) || Collider.CollideAt(Pos + new Vector2(-1, 0)))
                Velocity.X *= -1;

            if (!onGround)
                Gravity();
            else if (Velocity.Y > 0)
                Velocity.Y = 0;

            MoveX(Velocity.X * Platformer.Deltatime, null);
            MoveY(Velocity.Y * Platformer.Deltatime, null);
        }

        public override void Render()
        {
            Drawing.Draw(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), Color.Blue);

            if(Debug.DebugMode)
                Drawing.DrawEdge(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), 1, Color.Red);

        }
    }
}
