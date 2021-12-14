using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Components;

namespace Basic_platformer.Solids
{
    /// <summary>
    /// Acts as a Grappling Point that triggers something
    /// </summary>
    public class GrapplingTrigger : Solid
    {
        public event Action OnPulled;
        public float TimeToReactive;
        public bool Active;
        public GrapplingTrigger(Vector2 position, bool active, float timeToReactive, params Action[] triggeredActions) : base(position, 1, 1)
        {
            Pos = position;
            Active = active;
            TimeToReactive = timeToReactive;
            foreach (Action action in triggeredActions)
                OnPulled += action;
        }

        public void Pulled()
        {
            OnPulled();

            Active = false;
            if(TimeToReactive != 0)
                AddComponent(new Timer(TimeToReactive, true, null, () => Active = true));
        }
    }
}