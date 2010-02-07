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
        bool IsGameKeyDown(GameKey gamekey);
        bool WasKeyDown(Keys key);

        void Lock();
        void Unlock();
        bool Locked { get; }
        void SaveState();
    }

    public enum GameKey
    {
        RocketLauncher = Keys.Q,
        RailGun = Keys.E
    }

    public static class KeysExtensions
    {
        public static string GetStringValue(this Keys key)
        {
            switch (key)
            {
                // NUMERIC:
                case Keys.D0:
                case Keys.NumPad0:
                    return "0";
                case Keys.D1:
                case Keys.NumPad1:
                    return "1";
                case Keys.D2:
                case Keys.NumPad2:
                    return "2";
                case Keys.D3:
                case Keys.NumPad3:
                    return "3";
                case Keys.D4:
                case Keys.NumPad4:
                    return "4";
                case Keys.D5:
                case Keys.NumPad5:
                    return "5";
                case Keys.D6:
                case Keys.NumPad6:
                    return "6";
                case Keys.D7:
                case Keys.NumPad7:
                    return "7";
                case Keys.D8:
                case Keys.NumPad8:
                    return "8";
                case Keys.D9:
                case Keys.NumPad9:
                    return "9";
                // OTHER:
                case Keys.OemPeriod:
                    return ".";
                default:
                    return key.ToString();
            }
        }
    }

    public class AlphaNumericKeys : IEnumerable
    {
        private static Keys[] _keys = 
            {
                // ALPHA:
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, 
                // NUMERIC:
                Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0,
                Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.NumPad0,
                // OTHER:
                Keys.OemPeriod
            };

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _keys.Length; i++)
                yield return _keys[i];
        }

        #endregion

    }
}