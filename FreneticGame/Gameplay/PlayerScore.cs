using System;
using System.Linq;
using System.Collections.Generic;
using Frenetic.Player;

namespace Frenetic.Gameplay
{
    public class PlayerScore : IComparable
    {
        public int Kills { get; set; }
        public int Deaths { get; set; }

        public static bool operator>(PlayerScore lhs, PlayerScore rhs)
        {
            return ((lhs.Kills > rhs.Kills) || (lhs.Deaths < rhs.Deaths));
        }
        public static bool operator <(PlayerScore lhs, PlayerScore rhs)
        {
            return ((lhs.Kills < rhs.Kills) || (lhs.Deaths > rhs.Deaths));
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is PlayerScore)
            {
                PlayerScore rhs = (PlayerScore)obj;
                if (this < rhs) return -1;
                if (this > rhs) return 1;

                return 0;
            }
            else
            {
                throw new ArgumentException("Object is not a PlayerScore");
            }    
        }

        #endregion
    }
}
