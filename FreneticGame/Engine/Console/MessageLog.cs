using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Frenetic
{
    public class MessageLog : IEnumerable
    {
        public MessageLog(List<string> messageList)
        {
            _messageList = messageList;
        }
        public MessageLog()
        {
            _messageList = new List<string>();
        }

        public MessageLog Copy()
        {
            // NOTE: This only works with List<value type> (when it comes time to change, it might just be easier to see what's using Copy, and change them...)
            List<string> newList = new List<string>(_messageList);
            return new MessageLog(newList);
        }

        /// <summary>
        /// NOTE: This method is added just for the XmlSerializer. It's to be removed when we have better serialization. Rather use AddMessage(string message).
        /// </summary>
        public void Add(object justHereForSerializer)
        {
            AddMessage((string)justHereForSerializer);
        }

        public void AddMessage(string message)
        {
            _messageList.Insert(0, message);
        }

        public void BuildFromAnotherMessageLog(MessageLog sourceLog)
        {
            _messageList.Clear();
            
            foreach(string message in sourceLog)
            {
                // NOTE: This puts the newest message from sourceLog at index 0 (which is exactly what we want)
                _messageList.Add(message);
            }
        }

        public string StripOldestMessage()
        {
            string tmp = _messageList[_messageList.Count - 1];
            _messageList.RemoveAt(_messageList.Count - 1);
            return tmp;
        }

        #region List Methods/Properties
        public string this[int index] 
        {
            get
            {
                return _messageList[index];
            }
        }
        public bool TrueForAll(Predicate<string> match)
        {
            return _messageList.TrueForAll(match);
        }
        public int Count
        {
            get
            {
                return _messageList.Count;
            }
        }
        public bool Exists(Predicate<string> match)
        {
            return _messageList.Exists(match);
        }
        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            foreach (string message in _messageList)
                yield return message;
        }

        public IEnumerable<string> OldestToNewest
        {
            get
            {
                return _messageList.Reverse<string>();
            }
        }

        #endregion

        List<string> _messageList;
    }
}
