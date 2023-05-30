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
        private float acceleration = 0.1f;
        private float maxSpeed = 100f;
        public float Rotation = 0;

        private Player player;

        public HomingMissile(Vector2 position, float rotation) : base(position, 10, 5, 0, new Sprite(Color.OrangeRed))
        {
            Rotation = rotation;

            RemoveComponent(Collider);
            Collider = new BoxColliderRotated(-HalfSize, Width, Height, rotation, Vector2.Zero);
            AddComponent(Collider);

            Sprite.Origin = Vector2.One / 2; //To change when texture gets bigger
            Sprite.Rotation = MathHelper.ToRadians(Rotation);

            AddComponent(new Timer(5, true, null, SelfDestroy));

            AddComponent(new TrailRenderer(Vector2.Zero, 2));

            player = (Player)Engine.Player;
        }

        public override void Update()
        {
            acceleration = 6f;
            maxSpeed = 120f;

            Vector2 rotVec = (Engine.Player.MiddlePos - MiddlePos);
            if(rotVec != Vector2.Zero)
            {
                rotVec.Normalize();
                Rotation = VectorHelper.VectorToAngle(rotVec);
            }

            ((BoxColliderRotated)Collider).Rotation = Rotation;
            Sprite.Rotation = Rotation;

            Velocity += rotVec * acceleration;

            Velocity = new Vector2(Math.Clamp(Velocity.X, -maxSpeed, maxSpeed), Math.Clamp(Velocity.Y, -maxSpeed, maxSpeed));

            base.Update();

            Move(Velocity * Engine.Deltatime, SelfDestroy, SelfDestroy);

            if (Collider.Collide(player.Collider))
                SelfDestroy();
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

            if (Vector2.DistanceSquared(MiddlePos, player.MiddlePos) < 25 * 25)
                player.InstaDeath();
        }
    }
}
