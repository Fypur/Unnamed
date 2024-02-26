using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer
{
    public static class ServerManager
    {
        public static Server Server;

        public static void Start()
        {
            Server = new Server();
            Server.Start(7777, 4, 0, false);
            Server.MessageReceived += MessageReceived;
        }

        public static void Update()
        {
            Server.Update();
        }

        private static void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Server.SendToAll(e.Message, e.FromConnection.Id);
        }
    }
}
