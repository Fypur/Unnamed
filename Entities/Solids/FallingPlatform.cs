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
        private static readonly ParticleType Dust = Particles.Dust.Copy();

        private bool Collided;
        private bool previousOnGround;

        public FallingPlatform(Vector2 position, int width, int height, NineSliceSettings nineSlice)
            : base(position, width, height, new Sprite())
        {
            Trigger trig = (Trigger)AddChild(new Trigger(Pos - Vector2.UnitY, width, 1, new List<Type> { typeof(Player) }, Sprite.None));
            trig.OnTriggerEnterAction = (entity) => { AddComponent(new Coroutine(Fall(shakeTime))); RemoveChild(trig); };
            Sprite.NineSliceSettings = nineSlice;
            Dust.Acceleration = -Vector2.UnitY * 100;
        }

        private IEnumerator Fall(float shakeTime)
        {
            AddComponent(new Shaker(shakeTime, 1.2f, null, true));
            yield return new Coroutine.WaitForSeconds(shakeTime);
            gravityScale = constGravityScale;
        }

        public override void Update()
        {
            //TODO: Make this Reappear
            base.Update();
            Gravity();

            Action onCollision;
            if (!Collided)
            {
                onCollision = () =>
                {
                    Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, 100, new Rectangle((int)Pos.X, (int)(Pos.Y + Size.Y), Width, 2), null, 0, Color.White);
                    Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, 100, new Rectangle((int)Pos.X, (int)(Pos.Y + Size.Y), Width, 2), null, 180, Color.White);
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