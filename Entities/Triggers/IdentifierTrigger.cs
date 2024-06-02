using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnamed
{
    public class IdentifierTrigger : PlayerTrigger
    {
        public int Id;
        public bool PlayerIn => Contains(Engine.Player);
        public IdentifierTrigger(Vector2 position, Vector2 size, int id) : base(position, size, null)
        {
            Id = id;
        }
    }
}
