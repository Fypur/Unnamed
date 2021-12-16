using Basic_platformer.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Entities
{
    /// <summary>
    /// Class that can add or remove components
    /// </summary>
    public abstract class RenderedEntity : Entity
    {
        public List<Renderer> renderers = new List<Renderer>();

        public virtual void Render()
        {
            for (int i = renderers.Count - 1; i >= 0; i--)
                renderers[i].Render();
        }

        public override void AddComponent(Component component)
        {
            component.parentEntity = this;
            components.Add(component);

            if (component.GetType().IsSubclassOf(typeof(Renderer)))
                renderers.Add((Renderer)component);
        }

        public override void RemoveComponent(Component component)
        {
            components.Remove(component);

            if (component.GetType().IsSubclassOf(typeof(Renderer)))
                renderers.Remove((Renderer)component);
        }
    }
}
