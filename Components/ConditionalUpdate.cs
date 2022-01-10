using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class ConditionalUpdate : Component
    {
        private bool endBool;
        private Action onComplete;
        private Action updateAction;

        public ConditionalUpdate(bool EndBool, Action UpdateAction = null, Action OnComplete = null)
        {
            endBool = EndBool;
            onComplete = OnComplete;
            updateAction = UpdateAction;
        }

        public override void Update()
        {
            if (endBool)
                updateAction?.Invoke();
            else
            {
                onComplete?.Invoke();
                ParentEntity.RemoveComponent(this);
            }
        }

        public void End()
        {
            onComplete?.Invoke();
            ParentEntity.RemoveComponent(this);
        }
    }
}
