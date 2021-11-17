using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Solids
{
    public class MapData
    {
        public List<Solid> solids = new List<Solid>();
        public List<SolidTile> solidTiles = new List<SolidTile>();
        public List<GrapplingPoint> grapplingPoints = new List<GrapplingPoint>();

        public void AddElement(List<Solid> list)
        {

        }
    }
}