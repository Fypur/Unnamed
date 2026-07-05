using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class HomingMissile : Actor
    {
        private float acceleration = 6f;
        private float maxSpeed = 150f;

        private Player player;
        private Boss3 boss;
        private TrailRenderer trail;

        private bool canHitBoss;
        private BoxCollider boxColl;

        public HomingMissile(Vector2 position, float rotation) : base(position, new BoxCollider(-new Vector2(5, 2.5f), 10, 5, rotation, Vector2.Zero), new Sprite(DataManager.Objects["boss/missile"]))
        {
            Rotation = rotation;
            boxColl = (BoxCollider)Collider;

            Sprite.Origin = new Vector2(boxColl.Width, boxColl.Height) / 2;
            Sprite.Rotation = MathHelper.ToRadians(Rotation);

            AddComponent(new Timer(1, null, () => canHitBoss = true));
            AddComponent(new Timer(5, null, SelfDestroy));

            trail = (TrailRenderer)AddComponent(new TrailRenderer(Particles.FireTrail, Vector2.Zero, 0.01f));

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, Pos, 4);

            player = Platformer.Player;
            boss = Engine.CurrentMap.Data.GetEntity<Boss3>();
        }

        public override void Awake()
        {
            AddComponent(new Sound3D("SFX/Boss/Missile"));
        }

        public override void Update()
        {
            maxSpeed = 200f;

            Vector2 rotVec = (Platformer.Player.MiddlePos - MiddlePos);
            if (rotVec != Vector2.Zero)
            {
                rotVec.Normalize();
                Rotation = VectorHelper.ToAngleRad(rotVec);
            }

            BoxCollider colliderRotated = (BoxCollider)Collider;
            colliderRotated.Rotation = Rotation;
            Sprite.Rotation = Rotation;

            Velocity += rotVec * acceleration;

            float vLen = Velocity.Length();
            if (vLen > maxSpeed)
                Velocity = Velocity / vLen * maxSpeed;

            base.Update();


            MoveX(Velocity.X * Engine.Deltatime, (k) => SelfDestroy());
            MoveY(Velocity.Y * Engine.Deltatime, (k) => SelfDestroy());

            trail.LocalPosition = (colliderRotated.WorldVertices[3] + colliderRotated.WorldVertices[0]) / 2;
            trail.LocalPosition += (MiddlePos - trail.LocalPosition) / 3 - Pos;

            if (Collider.Collide(player.Collider))
            {
                player.Damage();
                SelfDestroy();
            }

            if (boss != null && canHitBoss && Collider.Collide(boss.Collider))
            {
                boss.Hit();
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

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Explosion, boxColl.Bounds, 100);
            Platformer.GameCam.Shake(0.4f, 1);

            AddComponent(new Sound3D("SFX/Boss/MissileExplode", autoRemove: true));

            /*if (Vector2.DistanceSquared(MiddlePos, player.MiddlePos) < 25 * 25)
                player.Damage();*/
        }
    }
}
