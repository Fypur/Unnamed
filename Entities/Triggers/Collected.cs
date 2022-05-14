using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class Collected : Collectable
    {
        public Collected(Vector2 position) : base(position, 10, 10, new Sprite(new Color(33, 185, 219)))
        { }

        public override IEnumerator OnCollected(Player player)
        {
            //TODO. Turn this into RAM bars
            Destroy();
            yield break;
        }
    }
}
