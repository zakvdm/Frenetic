using System;

namespace Frenetic.Graphics
{
    public enum PlayerTexture
    {
        Ball,
        Blank
    }
    public class XnaTextureBank<textureEnum> : ITextureBank<textureEnum>
    {
        public const string PlayerTextureDirectory = "Textures/";

        public XnaTextureBank(IContentManager contentManager)
        {
            _contentManager = contentManager;

            LoadTextures();
        }

        public ITexture this[textureEnum texture]
        {
            get
            {
                return _contentManager.Load<ITexture>(PlayerTextureDirectory + texture.ToString());
            }
        }

        private void LoadTextures()
        {
            foreach (string textureString in Enum.GetNames(typeof(textureEnum)))
            {
                _contentManager.Load<ITexture>(PlayerTextureDirectory + textureString);
            }
        }

        IContentManager _contentManager;
    }
}
