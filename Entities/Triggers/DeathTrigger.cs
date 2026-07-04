using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class DeathTrigger : Entity
    {
        public bool InstaDeath;
        public DeathTrigger(Vector2 position, HurtBox hurtBox) : base(position)
        {
            AddComponent(hurtBox);
        }

        public DeathTrigger(Vector2 position, Vector2 size)
            : this(position, new HurtBox(new AABBCollider(Vector2.Zero, (int)size.X, (int)size.Y))) { }
        public DeathTrigger(Vector2 position, int width, int height)
            : this(position, new HurtBox(new AABBCollider(Vector2.Zero, width, height))) { }
    }
}
