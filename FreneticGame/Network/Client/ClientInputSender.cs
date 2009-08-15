using System;
using Lidgren.Network;

namespace Frenetic.Network
{
    public class ClientInputSender : IView
    {
        public ClientInputSender(LocalClient localClient, Log<ChatMessage> chatLog, ISnapCounter snapCounter, IOutgoingMessageQueue outgoingMessageQueue)
        {
            this.LocalClient = localClient;
            this.ChatLog = chatLog;
            this.SnapCounter = snapCounter;
            this.OutgoingMessageQueue = outgoingMessageQueue;
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
            this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = this.LocalClient.ID, Type = ItemType.Player, Data = this.LocalClient.Player });

            this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = this.LocalClient.ID, Type = ItemType.PlayerSettings, Data = this.LocalClient.Player.PlayerSettings });
        }

        Log<ChatMessage> ChatLog;
        IOutgoingMessageQueue OutgoingMessageQueue;
        LocalClient LocalClient;
        ISnapCounter SnapCounter;

        int LastSentSnap = 0;
    }
}
