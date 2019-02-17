using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DraftMenu
{
    public class SelectCommanderOption : MenuOption
    {
        private readonly GameUnit unit;

        public SelectCommanderOption(GameUnit unit)
            : base(new RenderText(AssetManager.WindowFont, unit.Role.ToString()),
                TeamUtility.DetermineTeamColor(unit.Team))
        {
            this.unit = unit;
        }

        public override void Execute()
        {
            AssetManager.MenuConfirmSFX.Play();
            GameContext.DraftContext.SelectCommander(unit);
        }

        public override IRenderable Clone()
        {
            return new SelectCommanderOption(unit);
        }
    }
}