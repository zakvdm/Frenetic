using System;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Input;

using System.IO;
using System.Xml.Serialization;

namespace Frenetic
{
    public class NetworkManager
    {
        #region Networking Variables

        /// <summary>
        /// The network session for the game.
        /// </summary>
        private Microsoft.Xna.Framework.Net.NetworkSession networkSession;

        /// <summary>
        /// The packet writer for all of the data for the world.
        /// </summary>
        private PacketWriter packetWriter = new PacketWriter();

        /// <summary>
        /// The packet reader for all of the data for the world.
        /// </summary>
        private PacketReader packetReader = new PacketReader();

        private XmlSerializer playerSerializer = new XmlSerializer(typeof(OldPlayer));
        private XmlSerializer playerInputSerializer = new XmlSerializer(typeof(PlayerInput));
        private MemoryStream memoryStream = new MemoryStream();

        #endregion

        public NetworkManager(Microsoft.Xna.Framework.Net.NetworkSession networkSession)
        {
            if (networkSession == null)
                throw new ArgumentNullException("NetworkSession");

            this.networkSession = networkSession;

            //networkSession.SimulatedLatency = TimeSpan.FromMilliseconds(500);
        }

        #region Incoming Data
        /// <summary>
        /// This method reads all incoming packets and handles them appropriately
        ///</summary>
        public void ReadIncomingPackets()
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                while (gamer.IsDataAvailable)
                {
                    NetworkGamer sender;

                    // Read a single packet from the network.
                    gamer.ReceiveData(packetReader, out sender);

                    // Check packet type
                    WorldManager.PacketTypes packetType = (WorldManager.PacketTypes)packetReader.ReadInt32();

                    switch (packetType)
                    {
                        case WorldManager.PacketTypes.PlayerInput:
                            ReadInputFromClient(sender);
                            break;
                        case WorldManager.PacketTypes.GameState:
                            ReadGameStateFromServer();
                            break;
                    }
                }
            }
        }

        private void ReadInputFromClient(NetworkGamer sender)
        {
            // Look up the player object associated with whoever sent this packet
            OldPlayer player = sender.Tag as OldPlayer;

            //player.PlayerInput = new PlayerInput(packetReader);
            player.PlayerInput = (PlayerInput)playerInputSerializer.Deserialize(packetReader.BaseStream);
        }

        private void ReadGameStateFromServer()
        {
            // If a player has recently joined or left, it is possible the server
            // might have sent information about a different number of players
            // than the client currently knows about. If so, we will be unable
            // to match up which data refers to which player. The solution is
            // just to ignore the packet for now: this situation will resolve
            // itself as soon as the client gets the join/leave notification.
            if (networkSession.AllGamers.Count != packetReader.ReadInt32())
                return;

            // This packet contains data about all the players in the session.
            foreach (NetworkGamer remoteGamer in networkSession.AllGamers)
            {
                OldPlayer player = remoteGamer.Tag as OldPlayer;

                player = playerSerializer.Deserialize(packetReader.BaseStream) as OldPlayer;
            }
        }
        #endregion

        #region Outgoing Data
        public void SendOutgoingPackets(PlayerInput playerInput)
        {
            if (networkSession.IsHost)
            {
                SendGameState();
            }
            SendLocalInput(playerInput);
        }

        private void SendGameState()
        {
            packetWriter.Write((int)WorldManager.PacketTypes.GameState);

            // First off, our packet will indicate how many players it has data for.
            packetWriter.Write(networkSession.AllGamers.Count);

            // Loop over all the players in the session, not just the local ones!
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                // Look up what tank is associated with this player.
                OldPlayer player = gamer.Tag as OldPlayer;

                playerSerializer.Serialize(packetWriter.BaseStream, player);
            }

            // Send the combined data for all tanks to everyone in the session.
            LocalNetworkGamer server = (LocalNetworkGamer)networkSession.Host;

            server.SendData(packetWriter, SendDataOptions.InOrder);
        }

        private void SendLocalInput(PlayerInput playerInput)
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                //Player player = gamer.Tag as Player;

                // send out input data
                packetWriter.Write((int)WorldManager.PacketTypes.PlayerInput);
                playerInputSerializer.Serialize(packetWriter.BaseStream, playerInput);
                
                gamer.SendData(packetWriter, SendDataOptions.InOrder, networkSession.Host);
            }
        }
        #endregion
    }
}

