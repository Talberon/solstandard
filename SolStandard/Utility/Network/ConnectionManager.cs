using System;
using System.Diagnostics;
using System.Linq;
using Lidgren.Network;

namespace SolStandard.Utility.Network
{
    public class ConnectionManager
    {
        private NetServer server;
        private NetClient client;

        public void StartServer()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Sol Standard")
            {
                Port = 4444
            };

            if (client != null || server != null)
            {
                Trace.WriteLine("Server or client already started!");
                return;
            }

            Trace.WriteLine("Starting server!");
            server = new NetServer(config);
            server.Start();
        }

        public void StartClient(string host, int port)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Sol Standard");

            if (client != null || server != null)
            {
                Trace.WriteLine("Server or client already started!");
                return;
            }

            Trace.WriteLine("Starting client!");
            client = new NetClient(config);
            client.Start();
            client.Connect(host, port);
        }

        public void Listen()
        {
            if (client != null)
            {
                Listen(client);
            }

            if (server != null)
            {
                Listen(server);
            }
        }

        private static void Listen(NetPeer peer)
        {
            NetIncomingMessage received;
            while ((received = peer.ReadMessage()) != null)
            {
                switch (received.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        // handle custom messages
                        string data = received.ReadString();
                        Trace.WriteLine("RECEIVED:" + data);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        // handle connection status messages
                        switch (received.SenderConnection.Status)
                        {
                            case NetConnectionStatus.None:
                                break;
                            case NetConnectionStatus.InitiatedConnect:
                                Trace.WriteLine("Initiating connection...");
                                break;
                            case NetConnectionStatus.ReceivedInitiation:
                                Trace.WriteLine("Received Invitation...");
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                Trace.WriteLine("Awaiting Approval...");
                                break;
                            case NetConnectionStatus.RespondedConnect:
                                Trace.WriteLine("Connecting...");
                                break;
                            case NetConnectionStatus.Connected:
                                Trace.WriteLine("Connected!");
                                break;
                            case NetConnectionStatus.Disconnecting:
                                Trace.WriteLine("Disconnecting...");
                                break;
                            case NetConnectionStatus.Disconnected:
                                Trace.WriteLine("Disconnected!");
                                Trace.WriteLine(received.ReadString());
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;

                    case NetIncomingMessageType.DebugMessage:
                        // handle debug messages
                        // (only received when compiled in DEBUG mode)
                        Trace.WriteLine(received.ReadString());
                        break;

                    /* .. */
                    default:
                        Trace.WriteLine("unhandled message with type: " + received.MessageType);
                        Trace.WriteLine(received.ReadString());
                        break;
                }
            }
        }

        public void SendMessageAsClient(string textMessage)
        {
            Trace.WriteLine("Sending message to server!");
            NetOutgoingMessage message = client.CreateMessage();
            message.Write(textMessage);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendMessageAsServer(string textMessage)
        {
            Trace.WriteLine("Sending message to client!");
            NetOutgoingMessage message = server.CreateMessage();
            message.Write(textMessage);
            server.SendMessage(message, server.Connections.First(), NetDeliveryMethod.ReliableOrdered);
        }

        public void CloseServer()
        {
            if (server != null)
            {
                server.Shutdown("Closing server...");
                server = null;
            }
        }
    }
}