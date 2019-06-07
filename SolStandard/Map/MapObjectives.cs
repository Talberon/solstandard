using System.Collections.Generic;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.Utility;

namespace SolStandard.Map
{
    public class MapObjectives
    {
        private readonly bool modeAssassinate;
        private readonly bool modeRoutArmy;
        private readonly bool modeSeize;
        private readonly bool modeTaxes;
        private readonly bool modeSoloDefeatBoss;
        private readonly bool modeEscape;

        private readonly int valueTaxes;
        private readonly Team soloPlayerTeam;
        private readonly Team escapeTeam;

        public MapObjectives(bool modeAssassinate, bool modeRoutArmy, bool modeSeize, bool modeTaxes, int valueTaxes,
            bool modeSoloDefeatBoss, Team soloPlayerTeam, bool modeEscape, Team escapeTeam)
        {
            this.modeAssassinate = modeAssassinate;
            this.modeRoutArmy = modeRoutArmy;
            this.modeSeize = modeSeize;
            this.modeTaxes = modeTaxes;
            this.valueTaxes = valueTaxes;
            this.modeSoloDefeatBoss = modeSoloDefeatBoss;
            this.soloPlayerTeam = soloPlayerTeam;
            this.modeEscape = modeEscape;
            this.escapeTeam = escapeTeam;
        }

        public Scenario Scenario
        {
            get
            {
                Dictionary<VictoryConditions, Objective> objectives =
                    new Dictionary<VictoryConditions, Objective> {{VictoryConditions.Surrender, new Surrender()}};

                if (modeTaxes) objectives.Add(VictoryConditions.Taxes, new Taxes(valueTaxes));
                if (modeSeize) objectives.Add(VictoryConditions.Seize, new Seize());
                if (modeAssassinate) objectives.Add(VictoryConditions.Assassinate, new Assassinate());
                if (modeRoutArmy) objectives.Add(VictoryConditions.RoutArmy, new RoutArmy());
                if (modeEscape)
                {
                    objectives.Add(VictoryConditions.Escape,
                        new Escape(escapeTeam, (escapeTeam == Team.Blue) ? Team.Red : Team.Blue));
                }

                //SOLO OBJECTIVES
                if (modeSoloDefeatBoss)
                {
                    objectives.Add(VictoryConditions.SoloDefeatBoss, new SoloDefeatBoss(soloPlayerTeam));
                }

                return new Scenario(objectives);
            }
        }

        public static bool IsSoloGame(Scenario scenario)
        {
            return scenario.Objectives.ContainsKey(VictoryConditions.SoloDefeatBoss);
        }

        public static bool IsMultiplayerGame(Scenario scenario)
        {
            return !IsSoloGame(scenario);
        }

        public IRenderable Preview
        {
            get { return Scenario.ScenarioInfo(HorizontalAlignment.Centered); }
        }
    }
}