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
        private const float respawnTime = 3f;
        private static readonly ParticleType Dust = Particles.Dust.Copy();


        public bool Respawning;
        private Wipe wipe;
        private bool hasFallen;
        private bool previousOnGround;
        private Vector2 initPos;

        public FallingPlatform(Vector2 position, int width, int height, bool respawning, NineSlice nineSlice)
            : base(position, width, height, new Sprite())
        {
            TriggerComponent trig = (TriggerComponent)AddComponent(new TriggerComponent(-Vector2.UnitY, width, 1, new List<Type> { typeof(Player) }));
            trig.trigger.OnTriggerEnterAction = (entity) => { Fall(); trig.trigger.Active = false; };
            //trig.trigger.OnTriggerEnterAction = (entity) => { AddComponent(new Coroutine(FallEnumerator())); trig.trigger.Active = false; };
            
            Sprite.NineSliceSettings = nineSlice;
            Dust.Acceleration = -Vector2.UnitY * 100;
            initPos = Pos;
            Respawning = respawning;
        }

        public void Fall()
        {
            if (hasFallen)
                return;

            GetComponent<TriggerComponent>().trigger.Active = false;
            AddComponent(new Shaker(shakeTime, 1.2f, null, true));
            AddComponent(new Timer(shakeTime, true, null, () =>
            {
                gravityScale = constGravityScale;

                if (Respawning)
                {
                    AddComponent(new Timer(respawnTime, true, null, () => {
                        wipe = new Wipe(new Rectangle((initPos - Vector2.One).ToPoint(), (Size + Vector2.One * 2).ToPoint()), 1, Color.White, () => !Collider.CollideAt(Engine.Player, initPos), () =>
                        {
                            Pos = initPos; Velocity = Vector2.Zero; gravityScale = 0; previousOnGround = false; hasFallen = false;
                            GetComponent<TriggerComponent>().trigger.Active = true;
                        });
                        Engine.CurrentMap.Instantiate(wipe);
                    }));
                }
            }));
        }

        /*private IEnumerator FallEnumerator()
        {
            GetComponent<TriggerComponent>().trigger.Active = false;
            AddComponent(new Shaker(shakeTime, 1.2f, null, true));
            yield return new Coroutine.WaitForSeconds(shakeTime);
            gravityScale = constGravityScale;

            if (Respawning)
            {
                AddComponent(new Timer(respawnTime, true, null, () => {
                    wipe = new Wipe(new Rectangle((initPos - Vector2.One).ToPoint(), (Size + Vector2.One * 2).ToPoint()), 1, Color.White, () => !Collider.CollideAt(Engine.Player, initPos), () =>
                    {
                        Pos = initPos; Velocity = Vector2.Zero; gravityScale = 0; previousOnGround = false; Collided = false;
                        GetComponent<TriggerComponent>().trigger.Active = true;
                    });
                    Engine.CurrentMap.Instantiate(wipe);
                }));
            }
        }*/

        public override void Update()
        {
            if(!Collider.CollideAt(Pos + new Vector2(0, 1)))
                Gravity();

            Action onCollision;
            if (!hasFallen)
            {
                onCollision = () =>
                {
                    Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, 100, new Rectangle((int)Pos.X, (int)(Pos.Y + Size.Y), Width, 2), null, 0, Color.White);
                    Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, 100, new Rectangle((int)Pos.X, (int)(Pos.Y + Size.Y), Width, 2), null, 180, Color.White);
                    hasFallen = true;
                };
            }
            else
            {
                onCollision = null;
                if (!previousOnGround && Collider.CollideAt(Pos + new Vector2(0, 1)))
                    hasFallen = true;
            }

            Velocity.Y = Math.Min(Velocity.Y, maxFallingSpeed);
            MoveCollideSolids(Velocity * Engine.Deltatime, null, onCollision);

            if (hasFallen)
                previousOnGround = Collider.CollideAt(Pos + new Vector2(0, 1));

            base.Update();
            //Debug.LogUpdate(Pos - PreviousPos);
            if(Pos - PreviousExactPos != Vector2.Zero)
            { }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if(wipe != null)
                Engine.CurrentMap.Destroy(wipe);
        }
    }
}