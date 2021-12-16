using Basic_platformer.Solids;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;

namespace Basic_platformer.Mapping
{
    public class Level
    {
        public Vector2 Pos;
        public Vector2 Size;

        public Map ParentMap;
        public readonly int Index;
        private List<Entity> entityData;

        public Level(int index, Vector2 position, Map parentMap)
        {
            Pos = position;
            ParentMap = parentMap;
            entityData = LevelData.GetLevelData(index);
            Size = LevelData.GetLevelSize(index);
        }

        public void Load()
        {
            foreach(RenderedEntity e in entityData)
            {
                ParentMap.Data.RenderedEntities.Add(e);

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
            foreach (RenderedEntity e in entityData)
            {
                ParentMap.Data.RenderedEntities.Remove(e);

                if (e is Solid)
                    ParentMap.Data.Solids.Remove((Solid)e);
            }
        }
    }
}
