using Fiourp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public interface ISwinged
    {
        public virtual void OnGrapple(Entity grappledEntity, Func<bool> isAtSwingEnd) { }

        public virtual void OnStopGrapple(Entity unGrappledEntity) { }
    }
}
