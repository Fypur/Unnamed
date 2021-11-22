using Basic_platformer.Components;
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
        public List<Renderer> Renderers = new List<Renderer>();

        public virtual void Update()
        {
            for(int i = Components.Count - 1; i >= 0; i--)
                Components[i].Update();
        }

        public virtual void Render()
        {
            for (int i = Renderers.Count - 1; i >= 0; i--)
                Renderers[i].Render();
        }

        public void AddComponent(Component component)
        {
            component.parentEntity = this;
            Components.Add(component);

            if (component.GetType().IsSubclassOf(typeof(Renderer)))
                Renderers.Add((Renderer)component);
        }

        public void RemoveComponent(Component component)
        {
            Components.Remove(component);

            if (component.GetType().IsSubclassOf(typeof(Renderer)))
                Renderers.Remove((Renderer)component);
        }
    }
}
