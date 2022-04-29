using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class FallingPlatform : MovingSolid
    {
        private const float constGravityScale = 0.7f;
        private const float maxFallingSpeed = 160;
        private const float shakeTime = 0.5f;

        private bool Collided;
        private bool previousOnGround;

        public FallingPlatform(Vector2 position, int width, int height, NineSliceSettings nineSlice)
            : base(position, width, height, new Sprite())
        {
            TriggerComponent trig = (TriggerComponent)AddComponent(new TriggerComponent(-Vector2.UnitY, width, 1, new List<Type> { typeof(Player) }, null, null, null));
            trig.OnTriggerEnter = (entity) =>
            {
                AddComponent(new Coroutine(Fall(shakeTime)));
                RemoveComponent(trig);
            };

            Sprite.NineSliceSettings = nineSlice;
        }

        public IEnumerator Fall(float shakeTime)
        {
            AddComponent(new Shaker(shakeTime, 1.2f, null, true));
            yield return new Coroutine.WaitForSeconds(shakeTime);
            gravityScale = constGravityScale;
        }

        public override void Update()
        {
            base.Update();
            Gravity();

            Action onCollision;
            if (!Collided)
            {
                onCollision = () =>
                {
                    ParticleType dust = new ParticleType(Player.Dust);
                    dust.Acceleration = -Vector2.UnitY * 100;
                    Platformer.pS.Emit(dust, 100, new Rectangle((int)Pos.X, (int)(Pos.Y + Size.Y), Width, 2), null, 0, Color.White);
                    Platformer.pS.Emit(dust, 100, new Rectangle((int)Pos.X, (int)(Pos.Y + Size.Y), Width, 2), null, 180, Color.White);
                    Collided = true;
                };
            }
            else
            {
                onCollision = null;
                if (!previousOnGround && Collider.CollideAt(Pos + new Vector2(0, 1)))
                    Collided = true;
            }

            Velocity.Y = Math.Min(Velocity.Y, maxFallingSpeed);
            MoveCollideSolids(Velocity * Engine.Deltatime, null, onCollision);

            if (Collided)
                previousOnGround = Collider.CollideAt(Pos + new Vector2(0, 1));
        }
    }
}