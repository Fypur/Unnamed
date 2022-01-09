using Basic_platformer.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Entities
{
    public class Entity
    {
        public Vector2 Pos;
        public int Width;
        public int Height;

        public Vector2 HalfSize { get => new Vector2(Width / 2, Height / 2); }

        public Collider Collider;

        public List<Component> components = new List<Component>();
        public List<Renderer> renderers = new List<Renderer>();

        public Entity(Vector2 position, int width, int height)
        {
            Pos = position;
            Width = width;
            Height = height;
            Collider = new BoxCollider(Vector2.Zero, width, height);
            AddComponent(Collider);
        }

        public virtual void Update()
        {
            for (int i = components.Count - 1; i >= 0; i--)
                components[i].Update();
        }

        public virtual void Render()
        {
            for (int i = renderers.Count - 1; i >= 0; i--)
                renderers[i].Render();

            Collider.Render();
        }

        public void AddComponent(Component component)
        {
            component.parentEntity = this;
            components.Add(component);

            if (component.GetType().IsSubclassOf(typeof(Renderer)))
                renderers.Add((Renderer)component);
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);

            if (component.GetType().IsSubclassOf(typeof(Renderer)))
                renderers.Remove((Renderer)component);
        }
    }
}
