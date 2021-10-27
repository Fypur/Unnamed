using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public abstract class Component
    {
        public Entity parentEntity;

        public virtual void Added() { }
        public virtual void Removed() { }

        public virtual void Update() { }
    }
}