using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Goomba : Entity
    {
        private const float gravityScale = 100f;
        private const float speed = 100f;

        private bool onGround;

        public Goomba(Vector2 position, int width, int height) : base(position, width, height, gravityScale)
        {
            velocity.X = -speed;
        }

        public override void Update()
        {
            onGround = CollideAt(Platformer.Solids, Pos + new Vector2(0, 1));

            if (CollideAt(Platformer.Solids, Pos + new Vector2(1, 0)) || CollideAt(Platformer.Solids, Pos + new Vector2(-1, 0)))
                velocity.X *= -1;

            if (!onGround)
                Gravity();
            else if (velocity.Y > 0)
                velocity.Y = 0;
                
            MoveX(velocity.X * Platformer.Deltatime, null);
            MoveY(velocity.Y * Platformer.Deltatime, null);
            //TODO: Add DamagePlayer Method
        }

        public override void Render()
        {
            Drawing.Draw(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), Color.Red);
        }
    }
}
