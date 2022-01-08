using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;

namespace Basic_platformer.Triggers
{
    public abstract class Trigger : Entity
    {
        public Vector2 Size;
        public List<Type> Triggerers;
        private string name;

        private List<Entity> enteredEntities = new List<Entity>();

        public Trigger(Vector2 position, Vector2 size, List<Type> triggerers)
            : base(position, (int)size.X, (int)size.Y)
        {
            Pos = position; Size = size; Triggerers = triggerers; name = GetType().Name; 
        }

        public Trigger(Rectangle triggerRect, List<Type> triggerers)
            : base(triggerRect.Location.ToVector2(), triggerRect.Width, triggerRect.Height)
        {
            Pos = triggerRect.Location.ToVector2(); Size = triggerRect.Size.ToVector2(); Triggerers = triggerers; 
        }

        public override void Update()
        {
            base.Update();

            for (int i = Triggerers.Count - 1; i >= 0; i--)
                foreach (Actor actor in Platformer.CurrentMap.Data.EntitiesByType[Triggerers[i]])
                {
                    if (new Rectangle(actor.Pos.ToPoint(), new Point(actor.Width, actor.Height)).Intersects(new Rectangle(Pos.ToPoint(), Size.ToPoint())))
                    {
                        if (enteredEntities.Contains(actor))
                            OnTriggerStay(actor);
                        else
                        {
                            OnTriggerEnter(actor);
                            enteredEntities.Add(actor);
                        }
                    }
                    else if (enteredEntities.Contains(actor))
                    {
                        OnTriggerExit(actor);
                        enteredEntities.Remove(actor);
                    }
                }
        }

        public override void Render() 
        {
            Drawing.DrawString(name, Pos + Size / 2, Color.Aqua, true);
            Drawing.Draw(new Rectangle(Pos.ToPoint(), Size.ToPoint()), Color.Aqua * 0.2f);
        } 

        public virtual void OnTriggerEnter(Actor actor) { }

        public virtual void OnTriggerStay(Actor actor) { }

        public virtual void OnTriggerExit(Actor actor) { }
    }
}
