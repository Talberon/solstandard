using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events
{
    public class EndTurnEvent : IEvent
    {
        private readonly GameMapContext gameMapContext;

        public EndTurnEvent(ref GameMapContext gameMapContext)
        {
            this.gameMapContext = gameMapContext;
            Complete = false;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            MapContainer.ClearDynamicAndPreviewGrids();

            if (gameMapContext.SelectedUnit != null)
            {
                gameMapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
            }

            GameMapContext.SetPromptWindowText("Confirm End Turn");
            gameMapContext.CurrentTurnState = GameMapContext.TurnState.ResolvingTurn;
            Complete = true;
        }
    }
}