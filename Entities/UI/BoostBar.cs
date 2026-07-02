using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class BoostBar : Entity
    {
        public float Value;
        private Sprite filled;

        public BoostBar(Vector2 position, int width, int height, float value) : base(position)
        {
            AddComponent(filled = new Sprite(Color.Orange));

            AddComponent(new Sprite(Color.Gray));

            filled.Scale.X = value;
        }

        public override void Update()
        {
            base.Update();

            filled.Scale.X = Value;
        }
    }
}
