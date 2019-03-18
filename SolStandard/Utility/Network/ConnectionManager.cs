using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Lidgren.Network;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Buttons.Network;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;

namespace SolStandard.Utility.Network
{
    public class ConnectionManager
    {
        private NetServer server;
        private NetClient client;

        public const string PacketTypeHeader = "PT";
        public const int NetworkPort = 1993;

        public enum PacketType
        {
            Text,
            ControlInput,
            Event
        }

        public bool ConnectedAsServer
        {
            get
            {
                return server != null && server.ConnectionsCount > 0 &&
                       server.Connections.First().Status == NetConnectionStatus.Connected;
            }
        }

        public bool ConnectedAsClient
        {
            get { return client != null && client.ConnectionStatus == NetConnectionStatus.Connected; }
        }

        public IPAddress StartServer()
        {
            StopClientAndServer();

            NetPeerConfiguration config = new NetPeerConfiguration("Sol Standard")
            {
                Port = NetworkPort,
                EnableUPnP = true
            };

            Trace.WriteLine("Starting server!");
            server = new NetServer(config);

            try
            {
                server.Start();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }

            server.UPnP.ForwardPort(NetworkPort, "Forward Server Port for internet connections.");

            return server.UPnP.GetExternalIP();
        }

        public void StartClient(string host, int port)
        {
            StopClientAndServer();

            NetPeerConfiguration config = new NetPeerConfiguration("Sol Standard");

            Trace.WriteLine("Starting client!");
            client = new NetClient(config);
            client.Start();
            client.Connect(host, port);
        }

        private void StopClientAndServer()
        {
            if (client != null)
            {
                DisconnectClient();
            }

            if (server != null)
            {
                CloseServer();
            }
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

                        if (received.PeekString().Equals(""))
                        {
                            ReadNetworkEvent(received);
                        }
                        else
                        {
                            string data = received.ReadString();
                            Trace.WriteLine("RECEIVED:" + data);
                        }

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
                                //TODO Replace this with a unique seed
                                GameDriver.Random = new Random(12345);
                                GameContext.LoadMapSelect();

                                GameDriver.SetControllerConfig(peer is NetClient ? Team.Red : Team.Blue);
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

        private static void ReadNetworkEvent(NetBuffer received)
        {
            Trace.WriteLine("Reading network event...");
            byte[] messageBytes = received.ReadBytes(received.LengthBytes);

            using (Stream memoryStream = new MemoryStream(messageBytes))
            {
                IFormatter formatter = new BinaryFormatter();
                NetworkEvent receivedNetworkEvent = (NetworkEvent) formatter.Deserialize(memoryStream);
                Trace.WriteLine("Received event:" + receivedNetworkEvent);

                GlobalEventQueue.QueueSingleEvent(receivedNetworkEvent);
            }
        }

        public void SendTextMessageAsClient(string textMessage)
        {
            Trace.WriteLine("Sending text message to server!");
            NetOutgoingMessage message = client.CreateMessage();
            message.Write(textMessage);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendTextMessageAsServer(string textMessage)
        {
            Trace.WriteLine("Sending text message to client!");
            NetOutgoingMessage message = server.CreateMessage();
            message.Write(textMessage);
            server.SendMessage(message, server.Connections.First(), NetDeliveryMethod.ReliableOrdered);
        }

        public void SendControlMessageAsClient(NetworkController control)
        {
            Trace.WriteLine("Sending control message to server!");
            NetOutgoingMessage message = client.CreateMessage();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, control);
                byte[] controlBytes = memoryStream.ToArray();
                Trace.WriteLine(string.Format("Sending control message. Size: {0}", memoryStream.Length));
                message.Write(controlBytes);
                client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendControlMessageAsServer(NetworkController control)
        {
            Trace.WriteLine("Sending control message to client!");
            NetOutgoingMessage message = server.CreateMessage();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, control);
                byte[] controlBytes = memoryStream.ToArray();
                Trace.WriteLine(string.Format("Sending control message. Size: {0}", memoryStream.Length));
                message.Write(controlBytes);
                server.SendMessage(message, server.Connections.First(), NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendEventMessageAsClient(NetworkEvent networkEvent)
        {
            Trace.WriteLine("Sending event to server!");
            NetOutgoingMessage message = client.CreateMessage();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, networkEvent);
                byte[] controlBytes = memoryStream.ToArray();
                Trace.WriteLine(string.Format("Sending control message. Size: {0}", memoryStream.Length));
                message.Write(controlBytes);
                client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendEventMessageAsServer(NetworkEvent networkEvent)
        {
            Trace.WriteLine("Sending event to client!");
            NetOutgoingMessage message = server.CreateMessage();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, networkEvent);
                byte[] controlBytes = memoryStream.ToArray();
                Trace.WriteLine(string.Format("Sending control message. Size: {0}", memoryStream.Length));
                message.Write(controlBytes);
                server.SendMessage(message, server.Connections.First(), NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void CloseServer()
        {
            if (server == null) return;
            
            server.Shutdown("Closing server...");
            server = null;
        }

        public void DisconnectClient()
        {
            if (client == null) return;
            
            client.Shutdown("Shutting client connection down...");
            client = null;
        }
    }
}