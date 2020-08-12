using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DraftMenu
{
    public class SelectCommanderOption : MenuOption
    {
        private readonly GameUnit unit;
        private readonly UnitAction commandAction;

        public SelectCommanderOption(GameUnit unit, UnitAction commandAction)
            : base(WindowContent(unit, commandAction), TeamUtility.DetermineTeamWindowColor(unit.Team),
                HorizontalAlignment.Centered)
        {
            this.unit = unit;
            this.commandAction = commandAction;
        }

        private static IRenderable WindowContent(GameUnit unit, UnitAction commandAction)
        {
            return new WindowContentGrid(
                new[,]
                {
                    {
                        new Window.Window(
                            new WindowContentGrid(new[,]
                                {
                                    {unit.MediumPortrait},
                                    {new RenderText(AssetManager.StatFont, unit.Id)}
                                },
                                1,
                                HorizontalAlignment.Centered
                            ),
                            new Color(20, 20, 20, 100),
                            HorizontalAlignment.Centered
                        ) as IRenderable
                    },
                    {
                        new Window.Window(
                            new IRenderable[,]
                            {
                                {
                                    ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize,
                                        new Vector2(16)),
                                    new RenderText(
                                        AssetManager.SmallWindowFont, commandAction.Name,
                                        commandAction.FreeAction ? GlobalContext.PositiveColor : Color.White
                                    )
                                }
                            },
                            new Color(20, 20, 20, 180),
                            HorizontalAlignment.Centered
                        ) as IRenderable
                    }
                },
                1,
                HorizontalAlignment.Centered
            );
        }

        public override void Execute()
        {
            AssetManager.MenuConfirmSFX.Play();
            GlobalContext.DraftContext.SelectCommander(unit);
        }

        public override IRenderable Clone()
        {
            return new SelectCommanderOption(unit, commandAction);
        }
    }
}