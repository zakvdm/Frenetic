using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.Collections;

namespace Frenetic.UserInput
{
    public interface IKeyboard
    {
        bool IsKeyDown(Keys key);
        bool WasKeyDown(Keys key);

        void Lock();
        void Unlock();
        bool Locked { get; }
        void SaveState();
    }

    public class AlphaNumericKeys : IEnumerable
    {
        private static Keys[] _keys = { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
                                 Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z };

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            //return new AlphaNumericKeysEnumerator();
            for (int i = 0; i < _keys.Length; i++)
                yield return _keys[i];
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}