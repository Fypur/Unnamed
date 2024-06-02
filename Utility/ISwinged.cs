using Fiourp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnamed
{
    public interface ISwinged
    {
        public abstract float MaxSwingDistance { get; set; }

        public virtual void OnSwing(Entity grappledEntity, Func<bool> isAtSwingEnd) { }

        public virtual void OnStopSwing(Entity unGrappledEntity) { }
    }
}
