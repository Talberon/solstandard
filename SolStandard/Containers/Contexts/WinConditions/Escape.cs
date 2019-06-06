using System;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Escape : Objective
    {
        private Window objectiveWindow;
        private Team escapeTeam;
        private Team hunterTeam;

        public Escape(Team escapeTeam, Team hunterTeam)
        {
            this.escapeTeam = escapeTeam;
            this.hunterTeam = hunterTeam;
        }

        protected override IRenderable VictoryLabelContent
        {
            get { return new RenderText(AssetManager.ResultsFont, "ARMY ESCAPED"); }
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
                            ObjectiveIconProvider.GetObjectiveIcon(
                                VictoryConditions.Escape,
                                new Vector2(GameDriver.CellSize)
                            ),
                            new RenderText(AssetManager.WindowFont, "Escape"),
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
            //TODO Set up escape conditions
            //TODO Escaping player must have their commander exit the map via an escape point
            //TODO Hunter player must defeat the escaping commander before they escape
            //TODO Implement new Escape tile entity
            //TODO Set up asynchronous team sizes in Draft mode
            throw new NotImplementedException();
        }
    }
}