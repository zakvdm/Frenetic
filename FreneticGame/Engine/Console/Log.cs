using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Frenetic
{
    public class Log<T> : IEnumerable
    {
        public Log(List<T> messageList)
        {
            _messageList = messageList;
        }
        public Log()
        {
            _messageList = new List<T>();
        }

        /// <summary>
        /// NOTE: This method is added just for the XmlSerializer. It's to be removed when we have better serialization. Rather use AddMessage(string message).
        /// </summary>
        public void Add(object justHereForSerializer)
        {
            AddMessage((T)justHereForSerializer);
        }

        public void AddMessage(T message)
        {
            _messageList.Insert(0, message);
        }

        public T StripOldestMessage()
        {
            T tmp = _messageList[_messageList.Count - 1];
            _messageList.RemoveAt(_messageList.Count - 1);
            return tmp;
        }

        #region List Methods/Properties
        public T this[int index] 
        {
            get
            {
                return _messageList[index];
            }
        }
        public bool TrueForAll(Predicate<T> match)
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
        public bool Exists(Predicate<T> match)
        {
            return _messageList.Exists(match);
        }
        public void Clear()
        {
            _messageList.Clear();
        }
        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            foreach (T message in _messageList)
                yield return message;
        }

        public IEnumerable<T> OldestToNewest
        {
            get
            {
                return _messageList.Reverse<T>();
            }
        }

        #endregion

        List<T> _messageList;
    }
}
