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
    public class Key : Collectable
    {
        public Cage Cage;
        public Key(Vector2 position, Cage cage, Guid iid) : base(position, 10, 10, iid, new Sprite(Color.Yellow))
        {
            Cage = cage;
        }

        public override void OnCollected(Player player)
        {
            Cage.Unlock();
            SelfDestroy();
        }

        protected override bool CollectingConditions() => Vector2.DistanceSquared(MiddlePos, Cage.MiddlePos) < 4000;
    }
}
