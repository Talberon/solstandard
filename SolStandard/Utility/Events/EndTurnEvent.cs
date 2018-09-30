using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events
{
    public class EndTurnEvent : IEvent
    {
        private readonly MapContext mapContext;

        public EndTurnEvent(ref MapContext mapContext)
        {
            this.mapContext = mapContext;
            Complete = false;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            mapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
            mapContext.SetPromptWindowText("Confirm End Turn");
            mapContext.CurrentTurnState = MapContext.TurnState.ResolvingTurn;
            Complete = true;
        }
    }
}