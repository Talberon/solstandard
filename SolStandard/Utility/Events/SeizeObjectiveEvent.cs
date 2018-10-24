using System;
using System.Collections.Generic;
using System.Diagnostics;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class SeizeObjectiveEvent : IEvent
    {
        private readonly Team seizingTeam;

        public SeizeObjectiveEvent(Team seizingTeam)
        {
            this.seizingTeam = seizingTeam;
        }


        public bool Complete { get; private set; }

        public void Continue()
        {
            Seize seize = null;
            try
            {
                seize = GameContext.Scenario.Objectives[VictoryConditions.Seize] as Seize;
            }
            catch (KeyNotFoundException e)
            {
                Trace.TraceError("Seize could not be found in the victory conditions {0}", e);
                //TODO Use another SFX that's more specific to errors
                AssetManager.LockedSFX.Play();
                MapContainer.AddNewToastAtMapCursor("Seize is not a valid victory condition!", 500);
            }

            if (seize != null)
            {
                switch (seizingTeam)
                {
                    case Team.Red:
                        seize.RedSeizedObjective = true;
                        break;
                    case Team.Blue:
                        seize.BlueSeizedObjective = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            AssetManager.SkillBuffSFX.Play();
            Complete = true;
        }
    }
}