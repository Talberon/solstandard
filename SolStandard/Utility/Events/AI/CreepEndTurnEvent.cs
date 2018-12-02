using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;

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
            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}