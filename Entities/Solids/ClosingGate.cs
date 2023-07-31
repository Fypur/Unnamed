using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class ClosingGate : Solid
    {
        private int finalHeight;
        private Guid id;
        private static Dictionary<Guid, bool> closed = new();
        public ClosingGate(Vector2 position, int width, int height, Guid id) : base(position, width, 1, new Sprite(Color.Gray))
        {
            Collider.Collidable = false;
            finalHeight = height;
            this.id = id;
        }

        public void Close()
        {
            if (closed.TryGetValue(id, out bool open) && open)
                return;

            closed[id] = true;
            Collider.Collidable = true;

            AddComponent(new Timer(1, true, (timer) =>
            {
                Height = (int)MathHelper.Lerp(1, finalHeight, Ease.QuintIn(timer.AmountCompleted()));
            },
            () => Height = finalHeight));
        }
    }
}
