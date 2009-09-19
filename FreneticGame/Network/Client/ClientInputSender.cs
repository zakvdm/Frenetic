using System;
using Lidgren.Network;
using log4net;
using Frenetic.Engine;
using Frenetic.Player;

namespace Frenetic.Network
{
    public class ClientInputSender : IView
    {
        public ClientInputSender(LocalClient localClient, Log<ChatMessage> chatLog, ISnapCounter snapCounter, IOutgoingMessageQueue outgoingMessageQueue, ILoggerFactory loggerFactory)
        {
            this.LocalClient = localClient;
            this.ChatLog = chatLog;
            this.SnapCounter = snapCounter;
            this.OutgoingMessageQueue = outgoingMessageQueue;
            this.Logger = loggerFactory.GetLogger(this.GetType());
        }

        #region IView Members

        public void Generate()
        {
            // If the client ID is still 0 then we can't be connected yet, so no point in trying to send...
            if (this.LocalClient.ID == 0)
                return;

            if (this.SnapCounter.CurrentSnap > this.LastSentSnap)
            {
                this.LastSentSnap = this.SnapCounter.CurrentSnap;

                AddItemsToOutgoingMessageQueue();

                this.OutgoingMessageQueue.SendMessagesOnQueue();

                this.Logger.Debug("Sent ClientInput state for snap " + this.LastSentSnap);
                /*
                 * TODO:
                 *      GOOD GOD THIS IS UGLY!
                 *          This can NOT stay here. The solution is probably to send InputState which can be cleared every tick
                 */
                this.LocalClient.Player.PendingShot = null; // Only want to send this once...
            }
        }

        #endregion

        void AddItemsToOutgoingMessageQueue()
        {
            NewSendAllPendingChatMessages();
            SendLocalPlayerAndPlayerSettings();
        }

        void NewSendAllPendingChatMessages()
        {
            if (this.ChatLog.IsDirty)
            {
                this.OutgoingMessageQueue.AddToReliableQueue( new Item() { ClientID = this.LocalClient.ID, Type = ItemType.ChatLog, Data = this.ChatLog.GetDiff() });
                this.ChatLog.Clean();
            }
        }

        void SendLocalPlayerAndPlayerSettings()
        {
            IPlayerInput playerInput = new PlayerInput(this.LocalClient.Player);
            this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = this.LocalClient.ID, Type = ItemType.PlayerInput, Data = playerInput });

            if (this.LocalClient.Player.PlayerSettings.IsDirty)
            {
                this.OutgoingMessageQueue.AddToReliableQueue(new Item() { ClientID = this.LocalClient.ID, Type = ItemType.PlayerSettings, Data = this.LocalClient.Player.PlayerSettings.GetDiff() });
                this.LocalClient.Player.PlayerSettings.Clean();
            }
        }

        Log<ChatMessage> ChatLog;
        IOutgoingMessageQueue OutgoingMessageQueue;
        LocalClient LocalClient;
        ISnapCounter SnapCounter;
        ILog Logger;

        int LastSentSnap = 0;
    }
}
