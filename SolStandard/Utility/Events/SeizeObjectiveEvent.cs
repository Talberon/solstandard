using System;
using System.Collections.Generic;
using NLog;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Containers.Scenario.Objectives;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class SeizeObjectiveEvent : IEvent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                seize = GlobalContext.Scenario.Objectives[VictoryConditions.Seize] as Seize;
                AssetManager.SkillBuffSFX.Play();
            }
            catch (KeyNotFoundException e)
            {
                Logger.Error("Seize could not be found in the victory conditions {0}", e);
                AssetManager.ErrorSFX.Play();
                GlobalContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    "Seize is not a valid victory condition!", 50);
            }

            if (seize != null)
            {
                switch (seizingTeam)
                {
                    case Team.Red:
                        GlobalContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                            "Red team seizes the objective!", 100);
                        seize.RedSeizedObjective = true;
                        break;
                    case Team.Blue:
                        GlobalContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
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