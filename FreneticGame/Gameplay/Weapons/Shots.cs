using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Engine;

namespace Frenetic.Gameplay.Weapons
{
    public class Shots : List<Shot>, IDiffable<List<Shot>>
    {
        /*
         * NOTE: This implementation of IDiffable works pretty well. 
         *      Only one problem:  If we don't call Clean() regularly, the NewShots List fills up. We are going to duplicate data on the Client side (since the Client doesn't need Diffs...)
         */
        public new void Add(Shot shot)
        {
            NewShots.Add(shot);
            base.Add(shot);
        }
        #region IDiffable<List<Shot>> Members

        public void Clean()
        {
            NewShots.Clear();
        }

        public List<Shot> GetDiff()
        {
            return NewShots;
        }

        public bool IsDirty
        {
            get { return this.NewShots.Count > 0; }
        }

        List<Shot> NewShots = new List<Shot>();
        #endregion
    }
}
