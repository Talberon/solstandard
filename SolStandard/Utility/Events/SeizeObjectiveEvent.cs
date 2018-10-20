using System;
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
            Seize seize = GameContext.Scenario.Objectives[VictoryConditions.Seize] as Seize;
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