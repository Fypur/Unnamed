using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Entities
{
    public class Entity
    {
        protected List<Component> components = new List<Component>();

        public virtual void Update()
        {
            for (int i = components.Count - 1; i >= 0; i--)
                components[i].Update();
        }

        public virtual void AddComponent(Component component)
        {
            component.parentEntity = this;
            components.Add(component);
        }

        public virtual void RemoveComponent(Component component)
        {
            components.Remove(component);
        }
    }
}
