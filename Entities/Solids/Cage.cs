using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class Cage : Solid
    {
        public Cage(Vector2 position, int width, int height) : base(position, width, height, new Sprite(Color.Gray))
        {
        }

        public void Unlock()
        {
            SelfDestroy();
        }
    }
}
