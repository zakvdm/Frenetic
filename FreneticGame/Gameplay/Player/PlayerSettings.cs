using System;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Graphics;
using Frenetic.Engine;

namespace Frenetic.Player
{
    public class NetworkPlayerSettings : IPlayerSettings
    {
        public NetworkPlayerSettings()
        {
            Name = "NetworkPlayer";
            Texture = PlayerTexture.Ball;

            Random rnd = new Random();
            Color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        }

        [Tweakable]
        public string Name { get { return this._name; } set { this._name = value; DirtyState = true; } }
        [Tweakable]
        public Color Color { get { return this._color; } set { this._color = value; DirtyState = true; } }

        public PlayerTexture Texture { get { return this._playerTexture; } set { this._playerTexture = value; DirtyState = true; } }

        #region IDiffable<IPlayerSettings> Members

        public void Clean()
        {
            DirtyState = false;
        }

        public IPlayerSettings GetDiff()
        {
            return this;
        }

        public bool IsDirty
        {
            get { return this.DirtyState; }
        }

        bool DirtyState = false;

        #endregion

        string _name;
        Color _color;
        PlayerTexture _playerTexture;
    }

    public class LocalPlayerSettings : NetworkPlayerSettings
    {
        public LocalPlayerSettings()
        {
            Name = "Player";
            Texture = PlayerTexture.Ball;

            Color = Color.Black;
        }
/*
        [Tweakable]
        public string Name { get; set; }
        [Tweakable]
        public Color Color { get; set; }

        public PlayerTexture Texture { get; set; }
 */
    }
}
