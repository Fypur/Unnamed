using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class ScreenWipe : UIElement
    {
        public ScreenWipe(float wipeTime, Action onTransition = null, Action onThreeFourths = null, Action onEnd = null) : base(Engine.ScreenSize.OnlyX(), (int)Engine.ScreenSize.X, (int)Engine.ScreenSize.Y, new Sprite(Color.Black))
        {
            Overlay = true;
            Vector2 initPos = Pos;
            Vector2 endPos = Pos - Engine.ScreenSize.OnlyX() * 2;

            AddComponent(new Timer(wipeTime / 2, true, (timer) =>
            {
                float reversed = Ease.Reverse(timer.Value / wipeTime, 0.5f);
                float eased = Ease.QuintInAndOut(reversed);
                Pos = Vector2.Lerp(initPos, endPos, eased);
            },
            () =>
            {
                Pos = Vector2.Lerp(initPos, endPos, 0.5f);
                onTransition?.Invoke();

                bool doOnce = false;
                AddComponent(new Timer(wipeTime / 2, true, (timer) =>
                {
                    float reversed = 0.5f + Ease.Reverse((timer.Value) / wipeTime, 0.5f);
                    float eased = Ease.QuintInAndOut(reversed);
                    Pos = Vector2.Lerp(initPos, endPos, eased);

                    if (!doOnce && reversed >= 0.75f)
                    {
                        onThreeFourths?.Invoke();
                        doOnce = true;
                    }
                },
                () =>
                    {
                        onEnd?.Invoke();
                        Engine.CurrentMap.Destroy(this);
                    }));
            }));
        }
    }
}
