using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class Sawblade : CyclingSolid
    {
        public float Radius;
        public Sawblade(Vector2 position, float radius)
            : base(position, (int)(radius * 2), (int)(radius * 2), new Sprite(Color.Gray))
        {
            Radius = radius;
            Collider.Collidable = false;
            AddComponent(new HurtBox(new Vector2(radius), radius));
        }

        public Sawblade(Vector2 position, float radius, Vector2[] positions, float[] timesBetweenPositions, bool goingForwards)
            : base(position, (int)(radius * 2), (int)(radius * 2), new Sprite(Color.Gray), goingForwards, positions, timesBetweenPositions, Ease.QuintInAndOut)
        {
            Collider.Collidable = false;
            AddComponent(new HurtBox(new Vector2(radius / 2), radius));
        }

        public override void Awake()
        {
            RemoveComponent(Sprite);
            Sprite = null;
            base.Awake();
        }

        public override void Render()
        {
            base.Render();

            Drawing.DrawCircle(Collider.AbsolutePosition, Radius, 0.3f, Color.Gray);
        }
    }
}
