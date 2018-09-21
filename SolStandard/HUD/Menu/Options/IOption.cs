using SolStandard.Utility;

namespace SolStandard.HUD.Menu.Options
{
    public interface IOption
    {
        void Execute();
        string LabelText { get; }
        IRenderable OptionWindow { get; }
    }
}