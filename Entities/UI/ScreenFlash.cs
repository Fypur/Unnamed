using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{   
    public class ScreenFlash : UIElement
    {
        public ScreenFlash(float fadeOutTime, Func<float, float> ease) : base(Vector2.Zero, (int)(Engine.ScreenSize.X * Options.CurrentScreenSizeMultiplier), (int)(Engine.ScreenSize.Y * Options.CurrentScreenSizeMultiplier), new Sprite(new Color(Color.Gray, 25)))
        {
            Overlay = true;

            AddComponent(new Timer(fadeOutTime, true, (timer) =>
            {
                Sprite.Color *= (byte)(Ease.Reverse(ease(Ease.Reverse(timer.Value / timer.MaxValue))) * 25);
            },
            () =>
            {
                Sprite.Color.A = 0;
                SelfDestroy();
            }));
        }
    }
}
