using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DraftMenu
{
    public class SelectCommanderOption : MenuOption
    {
        private readonly GameUnit unit;

        public SelectCommanderOption(GameUnit unit)
            : base(WindowContent(unit), TeamUtility.DetermineTeamColor(unit.Team))
        {
            this.unit = unit;
        }

        private static IRenderable WindowContent(GameUnit unit)
        {
            return new WindowContentGrid(
                new[,]
                {
                    {unit.SmallPortrait},
                    {new RenderText(AssetManager.WindowFont, unit.Id)}
                },
                1,
                HorizontalAlignment.Centered
            );
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