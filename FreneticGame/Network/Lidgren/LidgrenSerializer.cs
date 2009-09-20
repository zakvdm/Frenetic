using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Player;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Frenetic.Weapons;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frenetic.Network.Lidgren
{
    public static class FreneticLidgrenExtensionSerialization
    {
        // MESSAGE:
        public static void Write(this NetBuffer netbuffer, Message message)
        {
            netbuffer.Write(message.Items.Count);
            foreach (Item item in message.Items)
            {
                netbuffer.Write(item);
            }
        }
        public static Message ReadMessage(this NetBuffer netbuffer)
        {
            var msg = new Message();

            int item_count = netbuffer.ReadInt32();
            for (int i = 0; i < item_count; i++)
            {
                msg.Items.Add(netbuffer.ReadItem());
            }

            return msg;
        }

        // ITEM:
        public static void Write(this NetBuffer netbuffer, Item item)
        {
            netbuffer.Write((ushort)item.Type);
            netbuffer.Write(item.ClientID);

            switch (item.Type)
            {
                case ItemType.PlayerInput:
                    netbuffer.Write((IPlayerInput)item.Data);
                    break;
                case ItemType.Player:
                    netbuffer.Write((IPlayerState)item.Data);
                    break;
                case ItemType.PlayerSettings:
                    netbuffer.Write((IPlayerSettings)item.Data);
                    break;
                case ItemType.ChatLog:
                    netbuffer.Write((List<ChatMessage>)item.Data);
                    break;
                case ItemType.NewClient:
                case ItemType.SuccessfulJoin:
                case ItemType.DisconnectingClient:
                    netbuffer.Write((int)item.Data);
                    break;
                default:
                    throw new NotImplementedException("Trying to serialize an unsupported type: " + item.Type.ToString());
            }
        }
        public static Item ReadItem(this NetBuffer netbuffer)
        {
            var item = new Item();

            item.Type = (ItemType)netbuffer.ReadUInt16();
            item.ClientID = netbuffer.ReadInt32();

            switch (item.Type)
            {
                case ItemType.PlayerInput:
                    item.Data = netbuffer.ReadPlayerInput();
                    break;
                case ItemType.Player:
                    item.Data = netbuffer.ReadPlayerState();
                    break;
                case ItemType.PlayerSettings:
                    item.Data = netbuffer.ReadPlayerSettings();
                    break;
                case ItemType.ChatLog:
                    item.Data = netbuffer.ReadChatLog();
                    break;
                case ItemType.NewClient:
                case ItemType.SuccessfulJoin:
                case ItemType.DisconnectingClient:
                    item.Data = netbuffer.ReadInt32();
                    break;
            }

            return item;
        }

        // PLAYERINPUT:
        public static void Write(this NetBuffer netbuffer, IPlayerInput input)
        {
            netbuffer.Write(input.Position);
            if (input.PendingStatus != null)
            {
                netbuffer.Write(true);
                netbuffer.Write((ushort)input.PendingStatus);
            }
            else
            {
                netbuffer.Write(false);
            }
            if (input.PendingShot != null)
            {
                netbuffer.Write(true);
                netbuffer.Write((Vector2)input.PendingShot);
            }
            else
            {
                netbuffer.Write(false);
            }
        }
        public static PlayerInput ReadPlayerInput(this NetBuffer netbuffer)
        {
            PlayerInput input = new PlayerInput();

            input.Position = netbuffer.ReadVector2();
            if (netbuffer.ReadBoolean())
            {
                input.PendingStatus = (PlayerStatus)netbuffer.ReadUInt16();
            }
            if (netbuffer.ReadBoolean())
            {
                input.PendingShot = netbuffer.ReadVector2();
            }

            return input;
        }

        // PLAYERSTATE:
        public static void Write(this NetBuffer netbuffer, IPlayerState state)
        {
            netbuffer.Write((ushort)state.Status);
            netbuffer.Write(state.NewShots.Count);
            foreach (Shot shot in state.NewShots)
            {
                netbuffer.Write(shot.StartPoint);
                netbuffer.Write(shot.EndPoint);
            }
            netbuffer.Write(state.Position);
            netbuffer.Write(state.Score.Deaths);
            netbuffer.Write(state.Score.Kills);
        }
        public static IPlayerState ReadPlayerState(this NetBuffer netbuffer)
        {
            PlayerState playerstate = new PlayerState();
           
            playerstate.Status = (PlayerStatus)netbuffer.ReadUInt16();
            int number_of_new_shots = netbuffer.ReadInt32();
            for (int i = 0; i < number_of_new_shots; i++)
            {
                Shot shot = new Shot();
                shot.StartPoint = netbuffer.ReadVector2();
                shot.EndPoint = netbuffer.ReadVector2();
                playerstate.NewShots.Add(shot);
            }
            playerstate.Position = netbuffer.ReadVector2();
            playerstate.Score.Deaths = netbuffer.ReadInt32();
            playerstate.Score.Kills = netbuffer.ReadInt32();

            return playerstate;
        }

        // PLAYERSETTINGS:
        public static void Write(this NetBuffer netbuffer, IPlayerSettings settings)
        {
            netbuffer.Write(settings.Color.PackedValue);
            netbuffer.Write(settings.Name);
            netbuffer.Write((ushort)settings.Texture);
        }
        public static IPlayerSettings ReadPlayerSettings(this NetBuffer netbuffer)
        {
            IPlayerSettings settings = new NetworkPlayerSettings();

            // TODO: Make IsDirty internal
            var color = new Color() { PackedValue = netbuffer.ReadUInt32() };
            settings.Color = color;
            settings.Name = netbuffer.ReadString();
            settings.Texture = (Frenetic.Graphics.PlayerTexture)netbuffer.ReadUInt16();

            return settings;
        }

        // CHATLOG:
        public static void Write(this NetBuffer netbuffer, List<ChatMessage> chatlog)
        {
            netbuffer.Write(chatlog.Count);
            foreach (ChatMessage msg in chatlog)
            {
                netbuffer.Write(msg.ClientName);
                netbuffer.Write(msg.Message);
            }
        }
        public static List<ChatMessage> ReadChatLog(this NetBuffer netbuffer)
        {
            var chatlog = new List<ChatMessage>();

            int message_count = netbuffer.ReadInt32();
            for (int i = 0; i < message_count; i++)
            {
                chatlog.Add(new ChatMessage() { ClientName = netbuffer.ReadString(), Message = netbuffer.ReadString() });
            }

            return chatlog;
        }
    }
}
