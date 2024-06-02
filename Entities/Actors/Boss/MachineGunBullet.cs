using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class MachineGunBullet : Actor
    {
        private float rotation;
        private float speed = 170f;
        public MachineGunBullet(Vector2 position, float rotation) : base(position, 10, 1, 0, new Sprite(Color.Yellow))
        {
            this.rotation = rotation;
            Sprite.Rotation = MathHelper.ToRadians(rotation);
            Sprite.Origin = Vector2.One / 2;

            RemoveComponent(Collider);
            Collider = new BoxColliderRotated(-HalfSize, Width, Height, rotation, Vector2.Zero);
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

            if (Collider.Collide(Engine.Player.Collider))
                ((Player)Engine.Player).Damage();
        }

        public override void Render()
        {
            base.Render();
        }

        private void Collision()
        {
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, Pos + Width * VectorHelper.AngleToVector(rotation));
            SelfDestroy();
        }
    }
}
