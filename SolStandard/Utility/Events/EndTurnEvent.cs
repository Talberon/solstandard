using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events
{
    public class EndTurnEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            MapContainer.ClearDynamicAndPreviewGrids();

            if (GameContext.GameMapContext.SelectedUnit != null)
            {
                GameContext.GameMapContext.SelectedUnit.SetUnitAnimation(UnitAnimationState.Idle);
            }

            GameMapContext.SetPromptWindowText("Confirm End Turn");
            GameContext.GameMapContext.CurrentTurnState = GameMapContext.TurnState.ResolvingTurn;
            Complete = true;
        }
    }
}