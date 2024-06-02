using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class Sawblade : MovingSolid
    {
        public float Radius;
        public Sawblade(Vector2 position, float radius)
            : base(position, (int)(radius * 2), (int)(radius * 2), new Sprite(DataManager.Objects["sawblade/sawblade"]))
        {
            Radius = radius;
        }

        public Sawblade(Vector2 position, float radius, Vector2[] positions, float[] timesBetweenPositions, bool goingForwards)
            : base(position, (int)(radius * 2), (int)(radius * 2), new Sprite(DataManager.Objects["sawblade/sawblade"]))
        {
            Radius = radius;
            AddComponent(new CycleMover(position, Width, Height, goingForwards, positions, timesBetweenPositions, Ease.CubeInAndOut, out Vector2 initPos));
            ExactPos = initPos;
        }

        public override void Awake()
        {
            base.Awake();

            Collider.Collidable = false;
            AddComponent(new HurtBox(HalfSize, Radius - 2));

            Sprite.Offset = HalfSize;
            Sprite.Origin = HalfSize;
        }

        public override void Update()
        {
            base.Update();

            Sprite.Rotation += 0.1f;
        }

        public override void Render()
        {
            base.Render();
        }
    }
}
