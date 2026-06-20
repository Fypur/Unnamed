using Fiourp;
using Microsoft.Xna.Framework;
using System;

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

        public GrapplingTrigger(Vector2 position, bool active, float timeToReactive, params Action[] triggeredActions) : base(position, new AABBCollider(Vector2.Zero, 1, 1), null)
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
            if (TimeToReactive != 0)
                AddComponent(new Timer(TimeToReactive, null, () => Active = true));
        }

        public override void Render()
        { }
    }
}