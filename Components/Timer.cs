using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Timer : Component
    {
        private readonly float maxValue;
        public float value;
        bool destroyOnComplete;
        Action onComplete;
        
        public Timer(float maxValue, bool destroyOnComplete = true, Action OnComplete = null)
        {
            this.maxValue = maxValue;
            value = maxValue;
            onComplete = OnComplete;
            this.destroyOnComplete = destroyOnComplete;
        }

        public override void Update()
        {
            if(value > 0)
            {
                value -= Platformer.Deltatime;
                if(value <= 0)
                {
                    value = 0;
                    onComplete?.Invoke();
                    if (destroyOnComplete)
                        parentEntity.RemoveComponent(this);
                }
            }
        }
    }
}