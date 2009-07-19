
namespace Frenetic.Graphics
{
    public interface IContentManager
    {
        T Load<T>(string assetName);
    }
}
