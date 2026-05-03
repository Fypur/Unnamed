using Fiourp;
using System;

namespace Unnamed
{
    public interface ISwinged
    {
        public abstract float MaxSwingDistance { get; set; }

        public virtual void OnSwing(Entity grappledEntity, Func<bool> isAtSwingEnd) { }

        public virtual void OnStopSwing(Entity unGrappledEntity) { }
    }
}
