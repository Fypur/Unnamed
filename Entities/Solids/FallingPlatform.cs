using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Unnamed
{
    public class FallingPlatform : Solid
    {
        private const float constGravityScale = 0.7f;
        private const float maxFallingSpeed = 160;
        private const float shakeTime = 0.5f;
        private const float respawnTime = 3f;
        private static readonly ParticleType Dust = Particles.Dust.Copy();

        private float gravityScale = 0;
        public bool Respawning;
        private Wipe wipe;
        public bool HasFallen;
        public bool HasFallenOnGround;
        private bool previousOnGround;
        private Vector2 initPos;

        private SpecialTrigger trig;

        public FallingPlatform(Vector2 position, int width, int height, bool respawning, NineSlice nineSlice)
            : base(position, new AABBCollider(Vector2.Zero, width, height), new Sprite())
        {
            trig = new SpecialTrigger(-Vector2.UnitY, width, 1, null);
            trig.OnTriggerEnterAction = (entity) => { Fall(); trig.Active = false; };

            Sprite.NineSliceSettings = nineSlice;
            Dust.Acceleration = -Vector2.UnitY * 100;
            initPos = Pos;
            Respawning = respawning;
        }

        public void Fall()
        {
            if (HasFallenOnGround)
                return;

            HasFallen = true;
            trig.Active = false;
            AddComponent(new Shaker(shakeTime, 1.2f, Sprite));
            AddComponent(new Timer(shakeTime, null, () =>
            {
                gravityScale = constGravityScale;

                if (Respawning)
                {
                    AddComponent(new Timer(respawnTime, null, () =>
                    {
                        wipe = new Wipe(new Rectangle((initPos - Vector2.One).ToPoint(), (AABBCollider.Size + Vector2.One * 2).ToPoint()), 1, Color.White, () => !CollideAt(Platformer.Player, initPos), () =>
                        {
                            Pos = initPos;
                            Velocity = Vector2.Zero;
                            gravityScale = 0;
                            previousOnGround = false;
                            HasFallenOnGround = false;
                            HasFallen = false;
                            trig.Active = true;
                        });
                        Engine.CurrentMap.Instantiate(wipe);
                    }));
                }
            }));
        }

        public override void Update()
        {
            if (!CollideAt(new List<Kinematic>(ParentMap.NonActorKinematics), Pos + new Vector2(0, 1)))
                Velocity.Y += 9.81f * gravityScale;

            Action onCollision;
            if (!HasFallenOnGround)
            {
                onCollision = () =>
                {
                    Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, 100, new Rectangle((int)Pos.X, (int)(Pos.Y + AABBCollider.Size.Y), AABBCollider.Width, 2), null, 0, Color.White);
                    Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, 100, new Rectangle((int)Pos.X, (int)(Pos.Y + AABBCollider.Size.Y), AABBCollider.Width, 2), null, 180, Color.White);
                    HasFallenOnGround = true;

                    AddComponent(new Sound3D("SFX/FallingBlock/Impact", autoRemove: true));
                };
            }
            else
            {
                onCollision = null;
                if (!previousOnGround && CollideAt(new List<Kinematic>(ParentMap.NonActorKinematics), Pos + new Vector2(0, 1)))
                    HasFallenOnGround = true;
            }

            Velocity.Y = Math.Min(Velocity.Y, maxFallingSpeed);
            MoveCollideSolids(Velocity * Engine.Deltatime, null, onCollision);

            if (HasFallenOnGround)
            {
                previousOnGround = CollideAt(new List<Kinematic>(ParentMap.NonActorKinematics), Pos + new Vector2(0, 1));
            }

            base.Update();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (wipe != null)
                Engine.CurrentMap.Destroy(wipe);
        }
    }
}