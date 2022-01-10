using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
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

            #region Entities By Type
            Type t = GetType();
            if (!Platformer.CurrentMap.Data.EntitiesByType.ContainsKey(t))
                Platformer.CurrentMap.Data.EntitiesByType.Add(t, new List<Entity>() { this });
            else
                Platformer.CurrentMap.Data.EntitiesByType[t].Add(this);
            #endregion

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

            if (Debug.DebugMode)
                Collider.Render();
        }

        public void AddComponent(Component component)
        {
            component.ParentEntity = this;
            component.Added();
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
