using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Timer : Component
    {
        public readonly float maxValue;
        public float value;
        bool destroyOnComplete;
        Action onComplete;
        Action<Timer> UpdateAction;
        
        public Timer(float maxValue, bool destroyOnComplete = true, Action OnComplete = null, Action<Timer> UpdateAction = null)
        {
            this.maxValue = maxValue;
            value = maxValue;
            onComplete = OnComplete;
            this.UpdateAction = UpdateAction;
            this.destroyOnComplete = destroyOnComplete;
        }

        public override void Update()
        {
            if(value > 0)
            {
                value -= Platformer.Deltatime;
                if (value <= 0)
                {
                    value = 0;
                    onComplete?.Invoke();
                    if (destroyOnComplete)
                        parentEntity.RemoveComponent(this);
                }
                else
                    UpdateAction?.Invoke(this);
            }
        }
    }
}