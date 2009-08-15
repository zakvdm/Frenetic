using System;
using Lidgren.Network;

namespace Frenetic.Network
{
    public class ClientInputSender : IView
    {
        public ClientInputSender(LocalClient localClient, IMessageConsole messageConsole, ISnapCounter snapCounter, IOutgoingMessageQueue outgoingMessageQueue)
        {
            this.LocalClient = localClient;
            this.MessageConsole = messageConsole;
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
            SendLastReceivedServerSnapAndCurrentClientSnap();
            SendAllPendingChatMessages();
            SendLocalPlayerAndPlayerSettings();
        }

        void SendLastReceivedServerSnapAndCurrentClientSnap()
        {
            // the last received server snap:
            this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = this.LocalClient.ID, Type = ItemType.ServerSnap, Data = this.LocalClient.LastServerSnap });

            // the current client snap:
            this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = this.LocalClient.ID, Type = ItemType.ClientSnap, Data = this.SnapCounter.CurrentSnap });
        }

        void SendAllPendingChatMessages()
        {
            // First. If there are new messages, we need to set the current client snap on them so we can keep sending them until this snap is acknowledged.
            foreach (ChatMessage newMsg in this.MessageConsole.UnsortedMessages)
            {
                newMsg.Snap = this.SnapCounter.CurrentSnap;
            }

            // NOTE: We use the LastClientSnap on the local client as the last client snap acknowledged by the server
            foreach (ChatMessage unAckedMsg in this.MessageConsole.GetPendingMessagesFromAfter(this.LocalClient.LastClientSnap))
            {
                this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = this.LocalClient.ID, Type = ItemType.ChatLog, Data = unAckedMsg });
            }
        }

        void SendLocalPlayerAndPlayerSettings()
        {
            this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = this.LocalClient.ID, Type = ItemType.Player, Data = this.LocalClient.Player });

            this.OutgoingMessageQueue.AddToQueue(new Item() { ClientID = this.LocalClient.ID, Type = ItemType.PlayerSettings, Data = this.LocalClient.Player.PlayerSettings });
        }

        IMessageConsole MessageConsole;
        IOutgoingMessageQueue OutgoingMessageQueue;
        LocalClient LocalClient;
        ISnapCounter SnapCounter;

        int LastSentSnap = 0;
    }
}
