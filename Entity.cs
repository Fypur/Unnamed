using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    /// <summary>
    /// Class that can add or remove components
    /// </summary>
    public abstract class Entity
    {
        public List<Component> Components = new List<Component>();
        private List<Component> componentsToAdd = new List<Component>();
        private List<Component> componentsToRemove = new List<Component>();

        public virtual void Update()
        {
            foreach (Component c in componentsToAdd)
                Components.Add(c);
            componentsToAdd.Clear();

            foreach (Component c in componentsToRemove)
                Components.Remove(c);
            componentsToRemove.Clear();

            foreach (Component c in Components)
                c.Update();
        }

        public void AddComponent(Component component)
        {
            component.parentEntity = this;
            componentsToAdd.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            componentsToRemove.Remove(component);
        }
    }
}
