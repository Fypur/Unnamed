using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Entities;
using Basic_platformer.Triggers;

namespace Basic_platformer.Mapping
{
    public class MapData
    {
        public Dictionary<Type, List<Actor>> EntitiesByType = new Dictionary<Type, List<Actor>>();
        
        public List<Entity> Entities = new List<Entity>();
        public List<Solid> Solids = new List<Solid>();
        public List<Actor> Actors = new List<Actor>();
        public List<Trigger> Triggers = new List<Trigger>();
        public List<Solid> GrapplingSolids = new List<Solid>();
    }
}