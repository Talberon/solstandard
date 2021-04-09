using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Lidgren.Network;
using NLog;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;

namespace SolStandard.Utility.Network
{
    public class ConnectionManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private NetServer server;
        private NetClient client;

        private static string AppIdentifier => $"Sol Standard v{GameDriver.VersionNumber} // " +
                                               $"CreepsEnabled:{CreepPreferences.Instance.CreepsCanSpawn}";

        public const int NetworkPort = 1993;

        public bool ConnectedAsServer =>
            server != null && server.ConnectionsCount > 0 &&
            server.Connections.First().Status == NetConnectionStatus.Connected;

        public bool ConnectedAsClient => client != null && client.ConnectionStatus == NetConnectionStatus.Connected;

        public string StartServer()
        {
            StopClientAndServer();

            var config = new NetPeerConfiguration(AppIdentifier)
            {
                Port = NetworkPort,
                EnableUPnP = true
            };

            Logger.Debug("Starting server!");
            server = new NetServer(config);

            try
            {
                server.Start();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }

            server.UPnP.ForwardPort(NetworkPort, "Forward Server Port for internet connections.");

            return GetExternalIP();
        }

        private static string GetExternalIP()
        {
            const string apiUrl = "https://ipinfo.io/ip";
            using var httpClient = new HttpClient();
            Task<string> responseString = httpClient.GetStringAsync(apiUrl);
            return responseString.Result.Trim();
        }

        public void StartClient(string host, int port)
        {
            StopClientAndServer();

            var config = new NetPeerConfiguration(AppIdentifier);

            Logger.Debug("Starting client!");
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

        private void Listen(NetPeer peer)
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
                            Logger.Debug("RECEIVED:" + data);
                        }

                        break;

                    case NetIncomingMessageType.StatusChanged:
                        // handle connection status messages
                        switch (received.SenderConnection.Status)
                        {
                            case NetConnectionStatus.None:
                                break;
                            case NetConnectionStatus.InitiatedConnect:
                                Logger.Debug("Initiating connection...");
                                GlobalHUDUtils.AddNotification("Initiating connection...");
                                break;
                            case NetConnectionStatus.ReceivedInitiation:
                                Logger.Debug("Received Initiation...");
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                Logger.Debug("Awaiting Approval...");
                                break;
                            case NetConnectionStatus.RespondedConnect:
                                Logger.Debug("Connecting...");
                                GlobalHUDUtils.AddNotification("Connecting...");
                                break;
                            case NetConnectionStatus.Connected:
                                Logger.Debug("Connected!");
                                GlobalHUDUtils.AddNotification("Connected to peer!");
                                ConnectionEstablished(peer);
                                break;
                            case NetConnectionStatus.Disconnecting:
                                Logger.Debug("Disconnecting...");
                                GlobalHUDUtils.AddNotification("Disconnecting...");
                                break;
                            case NetConnectionStatus.Disconnected:
                                Logger.Debug("Disconnected!");
                                Logger.Debug(received.ReadString());
                                GlobalHUDUtils.AddNotification("Disconnected from peer!");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;

                    case NetIncomingMessageType.DebugMessage:
                        // handle debug messages
                        // (only received when compiled in DEBUG mode)
                        Logger.Debug(received.ReadString());
                        break;

                    default:
                        Logger.Debug("unhandled message with type: " + received.MessageType);
                        Logger.Debug(received.ReadString());
                        break;
                }
            }
        }

        private void ConnectionEstablished(NetPeer peer)
        {
            GlobalContext.LoadMapSelect();

            if (ConnectedAsServer)
            {
                int newRandomSeed = GameDriver.Random.Next();
                GameDriver.Random = new Random(newRandomSeed);
                GlobalEventQueue.QueueSingleEvent(new InitializeRandomizerNet(newRandomSeed));
            }

            GameDriver.InitializeControlMappers(peer is NetClient ? Team.Red : Team.Blue);
        }

        private static void ReadNetworkEvent(NetBuffer received)
        {
            Logger.Debug("Reading network event...");
            byte[] messageBytes = received.ReadBytes(received.LengthBytes);

            using Stream memoryStream = new MemoryStream(messageBytes);
            IFormatter formatter = new BinaryFormatter();
            var receivedNetworkEvent = (NetworkEvent) formatter.Deserialize(memoryStream);
            Logger.Debug("Received event:" + receivedNetworkEvent);

            GlobalEventQueue.QueueSingleEvent(receivedNetworkEvent);
        }

        public void SendEventMessageAsClient(NetworkEvent networkEvent)
        {
            Logger.Debug("Sending event to server!");
            NetOutgoingMessage message = client.CreateMessage();

            using var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, networkEvent);
            byte[] controlBytes = memoryStream.ToArray();
            Logger.Debug($"Sending control message. Size: {memoryStream.Length}");
            message.Write(controlBytes);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendEventMessageAsServer(NetworkEvent networkEvent)
        {
            Logger.Debug("Sending event to client!");
            NetOutgoingMessage message = server.CreateMessage();

            using var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, networkEvent);
            byte[] controlBytes = memoryStream.ToArray();
            Logger.Debug($"Sending control message. Size: {memoryStream.Length}");
            message.Write(controlBytes);
            server.SendMessage(message, server.Connections.First(), NetDeliveryMethod.ReliableOrdered);
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

            client.Disconnect("Disconnecting from host...");
            client.Shutdown("Shutting client connection down...");
            client = null;
        }
    }
}