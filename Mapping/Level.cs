using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Unnamed
{
    public class Level
    {
        public readonly Map ParentMap;

        public Vector2 Pos;
        public Vector2 Size;
        public List<Entity> EntityData;
        public Action EnterAction = null;

        public Level(Vector2 position, Vector2 size, List<Entity> entityData, Action enterAction = null)
        {
            EntityData = entityData;
            Pos = position;
            Size = size;

            if (EntityData == null)
                EntityData = new List<Entity>();

            EnterAction = enterAction;
        }

        public void Load()
        {
            EnterAction?.Invoke();
            foreach (Entity entity in EntityData)
                ParentMap.Instantiate(entity);

        }

        public void Unload()
        {
            for (int i = EntityData.Count - 1; i >= 0; i--)
            {
                ParentMap.Destroy(EntityData[i]);
            }
        }

        public void DestroyOnUnload(Entity entity)
        {
            if (!EntityData.Contains(entity))
                EntityData.Add(entity);
        }

        public void DontDestroyOnUnload(Entity entity)
        {
            if (EntityData.Contains(entity))
                EntityData.Remove(entity);
        }
    }
}
