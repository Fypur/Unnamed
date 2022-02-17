using Fiourp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_platformer
{
    public interface ISwinged
    {
        public virtual void OnGrapple(Entity grappledEntity) { }

        public virtual void OnStopGrapple(Entity unGrappledEntity) { }
    }
}
