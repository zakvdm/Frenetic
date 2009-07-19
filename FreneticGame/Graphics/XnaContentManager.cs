using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Graphics
{
    class XnaContentManager : IContentManager
    {
        public ContentManager ContentManager { get; private set; }

        public XnaContentManager(ContentManager contentManager)
        {
            ContentManager = contentManager;
        }

        #region IContentManager Members

        public T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(ITexture))
            {
                return (T)(ITexture)new XnaTexture(ContentManager.Load<Texture2D>(assetName));
            }

            return default(T);
        }

        #endregion
    }
}
