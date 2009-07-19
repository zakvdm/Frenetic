using System;
using Lidgren.Network;
using System.Collections.Generic;

namespace Frenetic.Network.Lidgren
{
    public class LidgrenServerNetworkSession : IServerNetworkSession
    {
        public LidgrenServerNetworkSession(INetServer netServer, IServerMessageSender serverMessageSender, IMessageSerializer messageSerializer)
        {
            _netServer = netServer;
            _serverMessageSender = serverMessageSender;
            _messageSerializer = messageSerializer;

            ActiveConnections = new Dictionary<int, INetConnection>();
        }

        public event EventHandler<ClientStatusChangeEventArgs> ClientJoined;
        public event EventHandler<ClientStatusChangeEventArgs> ClientDisconnected;

        public void Dispose()
        {
            Shutdown("Cleaning up connection");
        }

        #region IServerNetworkSession Members

        public void Create(int port)
        {
            if (_netServer.IsListening)
                throw new System.InvalidOperationException("Session already created");

            _netServer.Port = port;
            _netServer.Start();
        }

        #region Sending
        public void SendTo(Message msg, NetChannel channel, int destinationClientID)
        {
            if ((!ActiveConnections.ContainsKey(destinationClientID)) || (ActiveConnections[destinationClientID].Status != NetConnectionStatus.Connected))
                throw new System.InvalidOperationException("Not a valid player");

            _serverMessageSender.SendTo(msg, channel, ActiveConnections[destinationClientID]);
        }
        public void SendToAll(Message msg, NetChannel channel)
        {
            _serverMessageSender.SendToAll(msg, channel);
        }
        public void SendToAllExcept(Message msg, NetChannel channel, int excludedClientID)
        {
            if (!ActiveConnections.ContainsKey(excludedClientID))
                throw new System.InvalidOperationException("Excluded player not connected to network session");

            _serverMessageSender.SendToAllExcept(msg, channel, ActiveConnections[excludedClientID]);
        }
        #endregion

        #endregion

        #region INetworkSession Members

        public void Shutdown(string reason)
        {
            _netServer.Shutdown(reason);
        }

        public Message ReadMessage()
        {
            NetBuffer inBuffer = _netServer.CreateBuffer();
            NetMessageType type;
            INetConnection sender;
            bool messageExists = false;
            messageExists = _netServer.ReadMessage(inBuffer, out type, out sender);
            if (messageExists)
            {
                switch (type)
                {
                    case NetMessageType.ConnectionApproval:
                        ApproveNewClient(sender);
                        break;
                    case NetMessageType.StatusChanged:
                        HandleClientStatusChanged(sender);
                        break;
                    case NetMessageType.DebugMessage:
                        Console.WriteLine(inBuffer.ReadString());
                        break;
                    case NetMessageType.Data:
                        return _messageSerializer.Deserialize(inBuffer.ReadBytes(inBuffer.LengthBytes));
                }
            }
            return null;
        }

        public Dictionary<int, INetConnection> ActiveConnections { get; private set; }

        #endregion

        void ApproveNewClient(INetConnection client)
        {
            client.Approve();
        }

        void HandleClientStatusChanged(INetConnection clientConnection)
        {
            Console.WriteLine("Status for " + clientConnection.ConnectionID.ToString() + " is: " + clientConnection.Status);
            if ((clientConnection.Status == NetConnectionStatus.Connected) && (!ActiveConnections.ContainsKey(clientConnection.ConnectionID)))
            {
                ActiveConnections.Add(clientConnection.ConnectionID, clientConnection);

                ProcessNewClient(clientConnection.ConnectionID);
            }
            else if ((clientConnection.Status == NetConnectionStatus.Disconnecting) && (ActiveConnections.ContainsKey(clientConnection.ConnectionID)))
            {
                ActiveConnections.Remove(clientConnection.ConnectionID);

                ProcessDisconnectingClient(clientConnection.ConnectionID);
            }
        }

        void ProcessNewClient(int newClientID)
        {
            if (ClientJoined != null)
                ClientJoined(this, new ClientStatusChangeEventArgs(newClientID, false));

            // send ack to new client:
            SendTo(new Message() { Type = MessageType.SuccessfulJoin, Data = newClientID }, NetChannel.ReliableInOrder1, newClientID);

            // send existent clients' info to new client:
            foreach (INetConnection connection in ActiveConnections.Values)
            {
                if (connection.ConnectionID == newClientID)
                    continue;   // We don't want to send the client to itself...

                SendTo(new Message() { Type = MessageType.NewClient, Data = connection.ConnectionID }, NetChannel.ReliableUnordered, newClientID);
            }
                
            // tell existent clients about new client:
            SendToAllExcept(new Message() { Type = MessageType.NewClient, Data = newClientID }, NetChannel.ReliableUnordered, newClientID);
        }
        void ProcessDisconnectingClient(int disconnectingClientID)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, new ClientStatusChangeEventArgs(disconnectingClientID, false));

            // tell all remaining clients about the disconnecting client
            SendToAll(new Message() { Type = MessageType.DisconnectingClient, Data = disconnectingClientID }, NetChannel.ReliableUnordered);
        }

        INetServer _netServer;
        IServerMessageSender _serverMessageSender;
        IMessageSerializer _messageSerializer;
    }
}
