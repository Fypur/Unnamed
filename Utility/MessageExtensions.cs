using Microsoft.Xna.Framework;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public static class MessageExtensions
    {
        public static Message AddVector2(this Message message, Vector2 value)
        {
            return message.AddFloat(value.X).AddFloat(value.Y);
        }

        public static Vector2 GetVector2(this Message message)
        {
            return new Vector2(message.GetFloat(), message.GetFloat());
        }
    }
}
