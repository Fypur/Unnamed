using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;

namespace Basic_platformer
{
    public abstract class Component
    {
        public Entity ParentEntity;

        public virtual void Added() { }
        public virtual void Removed() { }

        public virtual void Update() { }
    }
}