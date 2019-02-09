using Microsoft.Xna.Framework;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Seize : Objective
    {
        public bool RedSeizedObjective { get; set; }
        public bool BlueSeizedObjective { get; set; }
        private Window objectiveWindow;

        protected override IRenderable VictoryLabelContent
        {
            get { return new RenderText(AssetManager.ResultsFont, "Objective Seized!"); }
        }

        public override IRenderable ObjectiveInfo
        {
            get { return objectiveWindow ?? (objectiveWindow = BuildObjectiveWindow()); }
        }

        private static Window BuildObjectiveWindow()
        {
            return new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new SpriteAtlas(
                                AssetManager.ObjectiveIcons,
                                new Vector2(16),
                                new Vector2(GameDriver.CellSize),
                                (int) VictoryConditions.Seize,
                                Color.White
                            ),
                            new RenderText(AssetManager.WindowFont, "Seize Objective"),
                        }
                    },
                    2,
                    HorizontalAlignment.Centered
                ),
                ObjectiveWindowColor,
                HorizontalAlignment.Centered
            );
        }

        public override bool ConditionsMet()
        {
            if (RedSeizedObjective)
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            if (BlueSeizedObjective)
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            return false;
        }
    }
}