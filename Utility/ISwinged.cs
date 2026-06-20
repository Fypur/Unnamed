using Fiourp;
using System;

namespace Unnamed
{
    public interface ISwinged
    {
        public abstract float MaxSwingDistance { get; set; }

        public virtual void OnSwing(Kinematic grappledEntity, Func<bool> isAtSwingEnd) { }

        public virtual void OnStopSwing(Kinematic unGrappledEntity) { }
    }
}
