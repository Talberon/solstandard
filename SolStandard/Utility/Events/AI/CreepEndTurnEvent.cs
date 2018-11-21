using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events.AI
{
    public class CreepEndTurnEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            MapContainer.ClearDynamicAndPreviewGrids();

            if (GameContext.GameMapContext.SelectedUnit != null)
            {
                GameContext.GameMapContext.SelectedUnit.SetUnitAnimation(UnitAnimationState.Idle);
            }

            GameContext.GameMapContext.ResolveTurn();
            Complete = true;
        }
    }
}