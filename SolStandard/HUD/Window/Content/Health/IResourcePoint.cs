using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public interface IResourcePoint : IRenderable
    {
        bool Active { get; set; }
    }
}