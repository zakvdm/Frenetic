using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Frenetic.UserInput
{
    public interface IKeyMapping
    {
        FreneticKeys this[GameKey gamekey] { get; }
    }

    public class KeyMapping : IKeyMapping
    {
        public FreneticKeys this[GameKey gamekey]
        {
            get 
            {
                if (!this.Mapping.ContainsKey(gamekey))
                    this.Mapping.Add(gamekey, new FreneticKeys());

                return this.Mapping[gamekey];
            }
        }

        Dictionary<GameKey, FreneticKeys> Mapping = new Dictionary<GameKey, FreneticKeys>();
    }

}
