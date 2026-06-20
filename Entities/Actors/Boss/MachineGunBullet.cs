using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class MachineGunBullet : Actor
    {
        private const int length = 10;
        private const float speed = 170f;
        public MachineGunBullet(Vector2 position, float rotation) : base(position, new BoxCollider(-new Vector2(length / 2f, 0.5f), length, 1, rotation, Vector2.Zero), new Sprite(Color.Yellow))
        {
            Sprite.Rotation = MathHelper.ToRadians(rotation);
            Sprite.Origin = Vector2.One / 2;

            RemoveComponent(Collider);
            AddComponent(Collider);

            Velocity = VectorHelper.AngleToVector(rotation) * speed;

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, Pos, 4);
        }

        public override void Awake()
        {
            AddComponent(new Sound3D("SFX/Boss/MachineGun", autoRemove: true));
        }

        public override void Update()
        {
            Move(Velocity * Engine.Deltatime, Collision, Collision);

            base.Update();

            if (Collider.Collide(Platformer.Player.Collider))
                Platformer.Player.Damage();
        }

        public override void Render()
        {
            base.Render();
        }

        private void Collision()
        {
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, Pos + length * VectorHelper.AngleToVector(((BoxCollider)Collider).Rotation));
            SelfDestroy();
        }
    }
}
