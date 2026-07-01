using Fiourp;
using System;
using System.Collections.Generic;

namespace Unnamed
{
    public class Level
    {
        public readonly Map ParentMap;

        public List<Entity> EntityData;
        public Action EnterAction = null;

        public Level(List<Entity> entityData, Action enterAction = null)
        {
            EntityData = entityData;

            if (EntityData == null)
                EntityData = new List<Entity>();

            EnterAction = enterAction;
        }

        public void LoadNoAutoTile()
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
    }
}
