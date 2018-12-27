using System;
using Lidgren.Network;

namespace SolStandard.Utility.Network
{
    public class ConnectionManager
    {
        private NetServer server;

        public void StartServer()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Sol Standard")
            {
                Port = 12345
            };

            server = new NetServer(config);

            server.Start();

            StartListening();
        }

        private void StartListening()
        {
            NetIncomingMessage received;
            while ((received = server.ReadMessage()) != null)
            {
                switch (received.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        // handle custom messages
                        string data = received.ReadString();
                        Console.WriteLine("SERVER RECEIVED:" + data);

                        NetOutgoingMessage response = server.CreateMessage();
                        response.Write("Received message!");
                        server.SendMessage(response, received.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        // handle connection status messages
                        switch (received.SenderConnection.Status)
                        {
                            /* .. */
                        }

                        break;

                    case NetIncomingMessageType.DebugMessage:
                        // handle debug messages
                        // (only received when compiled in DEBUG mode)
                        Console.WriteLine(received.ReadString());
                        break;

                    /* .. */
                    default:
                        Console.WriteLine("unhandled message with type: "
                                          + received.MessageType);
                        break;
                }
            }
        }

        public void CloseServer()
        {
            if (server != null) server.Shutdown("Closing server...");
        }
    }
}