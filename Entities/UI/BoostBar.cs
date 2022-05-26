using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class BoostBar : UIElement
    {
        public float Value;
        private Sprite filled;
        public BoostBar(Vector2 position, int width, int height, float value) : base(position, width, height, new Sprite(Color.Orange), null)
        {
            Overlay = true;
            filled = GetComponent<Sprite>();
            AddComponent(new Sprite(Color.White));
            filled.desinationRectangle = Bounds;

            if (filled.desinationRectangle is Rectangle rect)
                rect.Width = (int)(width * value);
        }

        public override void Update()
        {
            base.Update();
            
            Debug.LogUpdate(Bounds, filled.desinationRectangle, Value);
            
            filled.desinationRectangle = new Rectangle(Bounds.Left, Bounds.Top, (int)(Width * Value), Height);
        }
    }
}
