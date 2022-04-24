using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Basic_platformer
{
    public class FallingPlatform : MovingSolid
    {
        private const float constGravityScale = 0.7f;
        private const float maxFallingSpeed = 160;
        private const float shakeTime = 0.5f;

        public FallingPlatform(Vector2 position, int width, int height)
            : base(position, width, height, new Sprite(Color.White))
        {
            TriggerComponent trig = (TriggerComponent)AddComponent(new TriggerComponent(-Vector2.UnitY, width, 1, new List<Type> { typeof(Player) }, null, null, null));
            trig.OnTriggerEnter = (entity) =>
            {
                AddComponent(new Coroutine(Fall(shakeTime)));
                RemoveComponent(trig);
            };
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

            Velocity.Y = Math.Min(Velocity.Y, maxFallingSpeed);
            MoveCollideSolids(Velocity * Engine.Deltatime);
        }
    }
}