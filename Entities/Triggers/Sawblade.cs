using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class Sawblade : Entity
    {
        public float Radius;
        private Sprite sprite;
        public Sawblade(Vector2 position, float radius)
            : base(position)
        {
            Radius = radius;
            AddComponent(sprite = new Sprite(DataManager.Objects["sawblade/sawblade"]));
            AddComponent(new HurtBox(new CircleCollider(new Vector2(radius), Radius - 2)));
        }

        public Sawblade(Vector2 position, float radius, Vector2[] positions, float[] timesBetweenPositions, bool goingForwards)
            : this(position, radius)
        {
            Radius = radius;
            AddComponent(new CycleMover(position, (int)(radius * 2), (int)(radius * 2), goingForwards, positions, timesBetweenPositions, Ease.CubeInAndOut, out Vector2 initPos));

            sprite.Offset = new Vector2(radius);
            sprite.Origin = new Vector2(radius);
        }

        public override void Update()
        {
            base.Update();

            sprite.Rotation += 0.1f;
        }

        public override void Render()
        {
            base.Render();
        }
    }
}
