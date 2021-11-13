using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Timer : Component
    {
        public readonly float MaxValue;
        public float Value;
        public float TimeScale = 1;

        private bool destroyOnComplete;

        Action onComplete;
        Action<Timer> UpdateAction;
        
        public Timer(float maxValue, bool destroyOnComplete = true, Action<Timer> UpdateAction = null, Action OnComplete = null)
        {
            this.MaxValue = maxValue;
            Value = maxValue;
            onComplete = OnComplete;
            this.UpdateAction = UpdateAction;
            this.destroyOnComplete = destroyOnComplete;
        }

        public override void Update()
        {
            if(Value > 0)
            {
                Value -= Platformer.Deltatime * TimeScale;
                if (Value <= 0)
                {
                    Value = 0;
                    onComplete?.Invoke();
                    if (destroyOnComplete)
                        parentEntity.RemoveComponent(this);
                }
                else
                    UpdateAction?.Invoke(this);
            }
        }

        public void End()
        {
            onComplete?.Invoke();
            parentEntity.RemoveComponent(this);
        }
    }
}