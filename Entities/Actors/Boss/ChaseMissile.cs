using Fiourp;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Platformer
{
    public class ChaseMissile : Actor
    {
        public float Rotation;

        private Player player;
        private TrailRenderer trail;

        public ChaseMissile(Vector2[] controlPoints, float time) : base(controlPoints[0], 10, 5, 0, new Sprite(DataManager.Objects["boss/missile"]))
        {
            player = (Player)Engine.Player;

            RemoveComponent(Collider);
            Collider = new BoxColliderRotated(-HalfSize, Width, Height, 0, Vector2.Zero);
            AddComponent(Collider);

            Sprite.Origin = HalfSize; //To change when texture gets bigger
            Sprite.Rotation = MathHelper.ToRadians(0);

            trail = (TrailRenderer)AddComponent(new TrailRenderer(Particles.FireTrail, Vector2.Zero, 0.01f));

            Vector2 initPos = Pos;
            AddComponent(new Timer(time, true, (timer) =>
            {
                Vector2 next = Bezier.Generic(controlPoints, timer.AmountCompleted());
                Rotation = VectorHelper.ToAngleRad(next - Pos);
                Velocity = (next - Pos) / Engine.Deltatime;
                Pos = next;
            },
            Explode));
        }

        public override void Awake()
        {
            AddComponent(new Timer(Rand.NextFloat(0, 0.2f), true, null, () => AddComponent(new Sound3D("SFX/Boss/Missile"))));
        }

        public override void Update()
        {
            BoxColliderRotated colliderRotated = (BoxColliderRotated)Collider;
            colliderRotated.Rotation = Rotation;
            Sprite.Rotation = Rotation;

            base.Update();


            trail.LocalPosition = (colliderRotated.Rect[3] + colliderRotated.Rect[0]) / 2;
            trail.LocalPosition += (MiddlePos - trail.LocalPosition) / 3 - Pos;


            if (Collider.Collide(player.Collider))
            {
                player.Damage();
                Explode();
            }
            else if(Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Platforms), Pos))
                Explode();
        }


        public void Explode()
        {
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Explosion, Bounds, 100);
            Engine.Cam.Shake(0.4f, 1);

            foreach (FallingPlatform falling in Engine.CurrentMap.Data.GetEntities<FallingPlatform>())
                if (Vector2.DistanceSquared(MiddlePos, falling.MiddlePos) < 10 * 10)
                    falling.Fall();

            AddComponent(new Sound3D("SFX/Boss/MissileExplode", autoRemove: true));

            SelfDestroy();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
