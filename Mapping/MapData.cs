using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Mapping
{
    public class MapData
    {
        public Dictionary<Type, List<Actor>> EntitiesByType = new Dictionary<Type, List<Actor>>();

        public List<Entity> Entities = new List<Entity>();
        public List<Solid> Solids = new List<Solid>();
        public List<Actor> Actors = new List<Actor>();
        public List<Solid> GrapplingSolids = new List<Solid>();
    }
}