﻿using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.General;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

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

                if (modeAssassinate) objectives.Add(VictoryConditions.Assassinate, new Assassinate());
                if (modeRoutArmy) objectives.Add(VictoryConditions.LastMan, new RoutArmy());
                if (modeSeize) objectives.Add(VictoryConditions.Seize, new Seize());
                if (modeTaxes) objectives.Add(VictoryConditions.Taxes, new Taxes(valueTaxes));


                return new Scenario(objectives);
            }
        }

        public IRenderable Preview
        {
            get
            {
                return new WindowContentGrid(new IRenderable[,]
                    {
                        {
                            new RenderText(AssetManager.WindowFont, (modeAssassinate) ? "Assassinate" : ""),
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (modeRoutArmy) ? "Rout Army" : ""),
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (modeSeize) ? "Seize Objective" : ""),
                        },
                        {
                            new RenderText(AssetManager.WindowFont,
                                (modeTaxes) ? "Collect Gold: " + valueTaxes + Currency.CurrencyAbbreviation : ""),
                        }
                    },
                    1
                );
            }
        }
    }
}