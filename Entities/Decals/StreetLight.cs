using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer.Entities.Decals
{
    public class StreetLight : Decoration
    {

        public StreetLight(Vector2 position, int width, int height) : base(position, width, height, new Sprite(DataManager.Objects["streetLight/light"]))
        {
            
        }
    }
}
