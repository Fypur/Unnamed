using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;

namespace Basic_platformer.Triggers
{
    public abstract class Trigger : Entity
    {
        public Vector2 Pos;
        public Vector2 Size;
        public List<Type> Triggerers;

        private List<RenderedEntity> enteredEntities;

        public Trigger(Vector2 position, Vector2 size, List<Type> triggerers) { Pos = position; Size = size; Triggerers = triggerers; }

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
                            OnTriggerEnter(actor);
                    }
                    else if (enteredEntities.Contains(actor))
                        OnTriggerExit(actor);
                }
        }

        public virtual void OnTriggerEnter(Actor actor) { }

        public virtual void OnTriggerStay(Actor actor) { }

        public virtual void OnTriggerExit(Actor actor) { }
    }
}
