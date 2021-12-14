using Basic_platformer.Solids;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Mapping
{
    public class Level
    {
        public Map ParentMap;
        public readonly int Index;
        private List<Entity> data;

        public Level(int index, Map parentMap)
        {
            ParentMap = parentMap;
            data = LevelData.GetLevelData(index);
        }

        public void Load()
        {
            foreach(Entity e in data)
            {
                ParentMap.Data.Entities.Add(e);

                if (e is Solid)
                {
                    ParentMap.Data.Solids.Add((Solid)e);

                    if (e is GrapplingTrigger || e is GrapplingPoint)
                        ParentMap.Data.GrapplingSolids.Add((Solid)e);
                }
                else if (e is Actor)
                    ParentMap.Data.Actors.Add((Actor)e);
            }
        }

        public void Unload()
        {
            foreach (Entity e in data)
            {
                ParentMap.Data.Entities.Remove(e);

                if (e is Solid)
                    ParentMap.Data.Solids.Remove((Solid)e);
            }
        }
    }
}
