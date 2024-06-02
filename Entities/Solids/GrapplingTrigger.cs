using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Fiourp;

namespace Unnamed
{
    /// <summary>
    /// Acts as a Grappling Point that triggers something
    /// </summary>
    public class GrapplingTrigger : Solid, ISwinged
    {
        public event Action OnPulled;
        public float TimeToReactive;

        public float MaxSwingDistance { get; set; }

        public GrapplingTrigger(Vector2 position, bool active, float timeToReactive, params Action[] triggeredActions) : base(position, 1, 1, null)
        {
            Active = active;
            TimeToReactive = timeToReactive;
            foreach (Action action in triggeredActions)
                OnPulled += action;

            SwingingPoint.SwingingPoints.Add(this);
            MaxSwingDistance = 1000;
        }

        public void Pulled()
        {
            OnPulled();

            Active = false;
            if(TimeToReactive != 0)
                AddComponent(new Timer(TimeToReactive, true, null, () => Active = true));
        }

        public override void Render()
        { }
    }
}