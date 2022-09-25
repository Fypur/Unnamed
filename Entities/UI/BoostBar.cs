using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class BoostBar : Entity
    {
        public float Value;
        private Sprite filled;

        public BoostBar(Vector2 position, int width, int height, float value) : base(position, width, height, new Sprite(Color.Orange))
        {
            filled = GetComponent<Sprite>();
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
