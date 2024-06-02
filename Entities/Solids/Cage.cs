using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
