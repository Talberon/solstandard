using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                AssetManager.SkillBuffSFX.Play();
            }
            catch (KeyNotFoundException e)
            {
                Trace.TraceError("Seize could not be found in the victory conditions {0}", e);
                AssetManager.ErrorSFX.Play();
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    "Seize is not a valid victory condition!", 50);
            }

            if (seize != null)
            {
                switch (seizingTeam)
                {
                    case Team.Red:
                        GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                            "Red team seizes the objective!", 100);
                        seize.RedSeizedObjective = true;
                        break;
                    case Team.Blue:
                        GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                            "Blue team seizes the objective!", 100);
                        seize.BlueSeizedObjective = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Complete = true;
        }
    }
}