using System.Collections.Generic;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Utility;

namespace SolStandard.Map
{
    public class MapObjectives
    {
        private readonly bool modeAssassinate;
        private readonly bool modeRoutArmy;
        private readonly bool modeSeize;
        private readonly bool modeTaxes;

        private readonly int valueTaxes;

        public MapObjectives(bool modeAssassinate, bool modeRoutArmy, bool modeSeize, bool modeTaxes, int valueTaxes)
        {
            this.modeAssassinate = modeAssassinate;
            this.modeRoutArmy = modeRoutArmy;
            this.modeSeize = modeSeize;
            this.modeTaxes = modeTaxes;
            this.valueTaxes = valueTaxes;
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


                return new Scenario(objectives);
            }
        }

        public IRenderable Preview
        {
            get { return Scenario.ScenarioInfo; }
        }
    }
}