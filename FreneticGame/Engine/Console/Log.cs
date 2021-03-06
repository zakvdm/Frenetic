﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Frenetic.Engine;

namespace Frenetic
{
    public class Log<T> : IEnumerable, IDiffable<List<T>>
    {
        public Log(List<T> messageList)
        {
            this.MessageList = messageList;
        }
        public Log()
        {
            this.MessageList = new List<T>();
        }

        public void Add(T message)
        {
            this.MessageList.Insert(0, message);
            this.DiffedList.Insert(0, message);
        }

        #region IDiffable
        // CACHE STUFF:
        public bool IsDirty
        {
            get
            {
                return this.DiffedList.Count > 0;
            }
        }
        public List<T> GetDiff()
        {
            return this.DiffedList;
        }
        public void Clean()
        {
            this.DiffedList = new List<T>();
        }
        #endregion

        public T StripOldestMessage()
        {
            T tmp = this.MessageList[this.MessageList.Count - 1];
            this.MessageList.RemoveAt(this.MessageList.Count - 1);
            return tmp;
        }

        #region List Methods/Properties
        public T this[int index] 
        {
            get
            {
                return this.MessageList[index];
            }
        }
        public bool TrueForAll(Predicate<T> match)
        {
            return this.MessageList.TrueForAll(match);
        }
        public int Count
        {
            get
            {
                return this.MessageList.Count;
            }
        }
        public bool Exists(Predicate<T> match)
        {
            return this.MessageList.Exists(match);
        }
        public void Clear()
        {
            this.MessageList.Clear();
        }
        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            foreach (T message in this.MessageList)
                yield return message;
        }

        public IEnumerable<T> OldestToNewest
        {
            get
            {
                return this.MessageList.Reverse<T>();
            }
        }

        #endregion

        List<T> MessageList;
        List<T> DiffedList = new List<T>();
    }
}
