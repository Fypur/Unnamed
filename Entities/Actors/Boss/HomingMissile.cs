using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class HomingMissile : Actor
    {
        private float acceleration = 6f;
        private float maxSpeed = 120f;
        public float Rotation = 0;

        private Player player;
        private Boss boss;
        private TrailRenderer trail;

        private bool canHitBoss;

        public HomingMissile(Vector2 position, float rotation) : base(position, 10, 5, 0, new Sprite(Color.OrangeRed))
        {
            Rotation = rotation;

            RemoveComponent(Collider);
            Collider = new BoxColliderRotated(-HalfSize, Width, Height, rotation, Vector2.Zero);
            AddComponent(Collider);

            Sprite.Origin = Vector2.One / 2; //To change when texture gets bigger
            Sprite.Rotation = MathHelper.ToRadians(Rotation);

            AddComponent(new Timer(1, true, null, () => canHitBoss = true));
            AddComponent(new Timer(5, true, null, SelfDestroy));

            trail = (TrailRenderer)AddComponent(new TrailRenderer(Vector2.Zero, 2));

            player = (Player)Engine.Player;
            boss = Engine.CurrentMap.Data.GetEntity<Boss>();
        }

        public override void Update()
        {
            acceleration = 6f;
            maxSpeed = 150f;

            Vector2 rotVec = (Engine.Player.MiddlePos - MiddlePos);
            if(rotVec != Vector2.Zero)
            {
                rotVec.Normalize();
                Rotation = VectorHelper.ToAngleRad(rotVec);
            }

            BoxColliderRotated colliderRotated = (BoxColliderRotated)Collider;
            colliderRotated.Rotation = Rotation;
            Sprite.Rotation = Rotation;

            Velocity += rotVec * acceleration;

            float vLen = Velocity.Length();
            if(vLen > maxSpeed)
                Velocity = Velocity / vLen * maxSpeed;

            base.Update();


            Move(Velocity * Engine.Deltatime, SelfDestroy, SelfDestroy);

            trail.LocalPosition = (colliderRotated.Rect[3] + colliderRotated.Rect[0]) / 2;
            trail.LocalPosition += (MiddlePos - trail.LocalPosition) / 3 - Pos;

            if (Collider.Collide(player.Collider))
            {
                player.Damage();
                SelfDestroy();
            }

            if (boss != null && canHitBoss && Collider.Collide(boss.Collider))
            {
                boss.Damage();
                SelfDestroy();
            }
        }

        public override void Render()
        {
            base.Render();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Explosion, Bounds, 100);
            Engine.Cam.Shake(0.4f, 1);

            /*if (Vector2.DistanceSquared(MiddlePos, player.MiddlePos) < 25 * 25)
                player.Damage();*/
        }
    }
}
