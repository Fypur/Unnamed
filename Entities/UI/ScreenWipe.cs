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
        private float wipeTime;
        private Action onTransition;
        private Action onThreeFourths;
        private Action onEnd;


        public ScreenWipe(float wipeTime, Action onTransition = null, Action onThreeFourths = null, Action onEnd = null) : base(new Vector2(1280, 0), 1280, 720, new Sprite(Color.Black))
        {
            this.wipeTime = wipeTime;
            this.onTransition = onTransition;
            this.onThreeFourths = onThreeFourths;
            this.onEnd = onEnd;
        }

        public override void Awake()
        {
            base.Awake();

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

        public override void Update()
        {
            base.Update();
            Debug.LogUpdate(Pos, Size);
        }
    }
}
