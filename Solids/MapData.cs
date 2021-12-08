using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Solids
{
    public class MapData
    {
        public List<Solid> Solids = new List<Solid>();
        public List<SolidTile> solidTiles = new List<SolidTile>();
        public List<Solid> GrapplingSolids = new List<Solid>();
        public List<Entity> Triggers = new List<Entity>();
    }
}