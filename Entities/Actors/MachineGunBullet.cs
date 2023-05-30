using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
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
        }

        public override void Update()
        {
            Move(Velocity * Engine.Deltatime, Collision, Collision);

            base.Update();

            if (Collider.Collide(Engine.Player.Collider))
                ((Player)Engine.Player).InstaDeath();
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
