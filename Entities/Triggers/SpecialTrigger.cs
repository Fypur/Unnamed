using Microsoft.Xna.Framework;
using System;

namespace Unnamed
{
    public class SpecialTrigger : PlayerTrigger
    {
        public Action<Player> OnTriggerEnterAction;
        public SpecialTrigger(Vector2 position, int width, int height, Action<Player> onTriggerEnter) : base(position, width, height, null)
        {
            this.OnTriggerEnterAction = onTriggerEnter;
        }

        public SpecialTrigger(Vector2 position, Vector2 size, Action<Player> onTriggerEnter) : this(position, (int)size.X, (int)size.Y, onTriggerEnter)
        { }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);

            OnTriggerEnterAction?.Invoke(player);
        }
    }
}
