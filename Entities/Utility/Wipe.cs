using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class Wipe : Entity
    {
        public float Progress;
        public Vector2 initPos;
        public Vector2 endPos;

        private Rectangle wiped;
        private float wipeTime;
        private Action onTransition;
        private Action onEnd;
        private Action onThreeFourths;

        public bool Paused { get; private set; }

        public Wipe(Rectangle wiped, float wipeTime, Color color, Func<bool> pausedUntil = null, Action onTransition = null, Action onThreeFourths = null, Action onEnd = null) : 
            base(wiped.Location.ToVector2() - new Vector2(wiped.Width, 0), wiped.Width, wiped.Height, new Sprite(color))
        {
            Layer = 3;
            Pos = wiped.Location.ToVector2();

            this.wiped = wiped;
            this.wipeTime = wipeTime;
            if(pausedUntil != null)
                this.Paused = pausedUntil();
            this.onTransition = onTransition;
            this.onEnd = onEnd;
            this.onThreeFourths = onThreeFourths;

            initPos = wiped.Location.ToVector2() - new Vector2(wiped.Width, 0);
            endPos = wiped.Location.ToVector2() + new Vector2(Width, 0);

            Vector2 usedPos = initPos;
            Width = 0;

            AddComponent(new Timer(wipeTime / 2, true, (timer) =>
            {
                float reversed = Ease.Reverse(timer.Value / wipeTime, 0.5f);
                Progress = Ease.CubeInAndOut(reversed);
                usedPos = Vector2.Lerp(initPos, endPos, Progress);
                //Pos = initPos + new Vector2(wiped.Width, 0);
                Width = (int)Math.Ceiling(wiped.Width * Progress * 2);
            },
            () =>
            {
                usedPos = Vector2.Lerp(initPos, endPos, 0.5f);
                Width = wiped.Width;

                bool hasOnTranstionned = false;
                bool doOnce = false;
                AddComponent(new Timer(wipeTime / 2, true, (timer) =>
                {
                    if(pausedUntil != null)
                    {
                        timer.PauseUntil(pausedUntil);
                        if (!pausedUntil())
                            return;
                    }

                    if (!hasOnTranstionned)
                    {
                        onTransition?.Invoke();
                        hasOnTranstionned = true;
                    }

                    float reversed = 0.5f + Ease.Reverse((timer.Value) / wipeTime, 0.5f);
                    if (!doOnce && reversed >= 0.75f)
                    {
                        onThreeFourths?.Invoke();
                        doOnce = true;
                    }
                    Progress = Ease.CubeInAndOut(reversed);
                    usedPos = Vector2.Lerp(initPos, endPos, Progress);
                    Pos.X = (int)Math.Round(usedPos.X);
                    //Width = (int)Math.Ceiling(wiped.Width * Ease.Reverse(Progress - 0.5f, 0.5f) * 2);
                    Width = (int)Math.Ceiling(endPos.X - Pos.X);
                },
                () =>
                {
                    onEnd?.Invoke();
                    SelfDestroy();
                }));
            }));
        }

        public void Resume() 
        { 
            if(Paused == false)
                return;
            Paused = false;
        }
    }
}
