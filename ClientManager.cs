using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public static class ClientManager
    {
        public static Client LocalClient;
        public static Dictionary<ushort, Player> Players = new();
        public static Player LocalPlayer => Players[LocalClient.Id];
        public static void Connect()
        {
            LocalClient = new Client();

            LocalClient.Connected += OnConnect;
            LocalClient.ClientDisconnected += OnClientDisconnect;

            LocalClient.Connect("127.0.0.1:7777");
        }

        public static void Update()
        {
            LocalClient.Update();

            if (!LocalClient.IsConnected)
                return;

            UpdateInput();
            SendInfo();
        }

        public static void UpdateInput()
        {
            LocalPlayer.Inputs.CopyTo(LocalPlayer.PrevInputs, 0);
            MultiplayerInput.SetLocalInputs(LocalPlayer.Inputs);
        }

        private static void OnConnect(object sender, EventArgs e)
        {
            Players[LocalClient.Id] = (Player)Engine.Player;

            /*Message spawnMessage = Message.Create(MessageSendMode.Reliable, IDS.Spawn);
            spawnMessage.AddUShort(LocalClient.Id);
            spawnMessage.AddVector2(LocalPlayer.ExactPos);
            LocalClient.Send(spawnMessage);*/
        }

        private static void OnClientDisconnect(object sender, ClientDisconnectedEventArgs args)
        {
            Engine.CurrentMap.Destroy(Players[args.Id]);
            Players.Remove(args.Id);
        }

        /*[MessageHandler((ushort)IDS.Spawn)]
        private static void SpawnOther()
        {
            ushort id = message.GetUShort();
            Vector2 pos = message.GetVector2();

            Players[id] = (Player)Engine.CurrentMap.Instantiate(new Player(pos));
        }*/

        private static void SendInfo()
        {
            Message message = Message.Create(MessageSendMode.Unreliable, IDS.PlayerInfo);
            message.AddUShort(LocalClient.Id);
            message.AddVector2(LocalPlayer.ExactPos);
            message.AddVector2(LocalPlayer.Velocity);
            message.AddInputs(LocalPlayer.Inputs);
            LocalClient.Send(message);
        }

        [MessageHandler((ushort)IDS.PlayerInfo)]
        private static void HandlePlayerInfo(Message message)
        {
            ushort id = message.GetUShort();
            Vector2 pos = message.GetVector2();
            Vector2 velocity = message.GetVector2();

            if (!Players.ContainsKey(id))
                Players[id] = (Player)Engine.CurrentMap.Instantiate(new Player(pos));

            Players[id].ExactPos = pos;
            Players[id].Velocity = velocity;
            Players[id].Inputs.CopyTo(Players[id].PrevInputs, 0);
            message.GetInputs(Players[id].Inputs);
        }

        public static void OnExit()
        {
            if (LocalClient.IsConnected)
                LocalClient.Disconnect();
        }
    }
}
