using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Unnamed
{
    public class ClosingGate : Solid
    {
        private int finalHeight;
        private Guid id;
        public static Dictionary<Guid, bool> ClosedGates = new();
        bool closed = false;
        public ClosingGate(Vector2 position, int width, int height, Guid id) : base(position, new AABBCollider(Vector2.Zero, width, 1), new Sprite(DataManager.Objects["closingGate1"]))
        {
            Collider.Collidable = false;
            finalHeight = height;
            this.id = id;

            Sprite.Add(Sprite.AllAnimData["ClosingGate"]);

            if (ClosedGates.TryGetValue(id, out bool closed) && closed)
                Close();
        }

        public void Close()
        {
            if (closed)
                return;

            Sprite.Play("close");

            closed = true;
            ClosedGates[id] = true;
            Collider.Collidable = true;

            AddComponent(new Timer(1, (timer) =>
            {
                AABBCollider.Height = (int)MathHelper.Lerp(1, finalHeight, Ease.QuintIn(timer.AmountCompleted()));
            },
            () => AABBCollider.Height = finalHeight));
        }
    }
}
