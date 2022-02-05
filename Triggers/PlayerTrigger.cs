using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_platformer
{
    public abstract class PlayerTrigger : Trigger
    {
        public PlayerTrigger(Vector2 position, Vector2 size, Sprite sprite)
            : base(position, (int)size.X, (int)size.Y, new List<Type> { typeof(Player) }, sprite) { }

        public PlayerTrigger(Vector2 position, int width, int height, Sprite sprite)
            : base(position, new Vector2(width, height), new List<Type> { typeof(Player) }, sprite) { }

        public PlayerTrigger(Rectangle triggerRect, Sprite sprite)
            : base(triggerRect.Location.ToVector2(), triggerRect.Size.ToVector2(), new List<Type> { typeof(Player) }, sprite) { }
    }
}
