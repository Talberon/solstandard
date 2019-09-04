using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity;
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

            //IMPORTANT Do not allow tiles that have been triggered to trigger again or the risk of soft-locking via infinite triggers can occur
            if (!GameMapContext.TriggerEffectTiles(EffectTriggerTime.EndOfTurn, true))
            {
                GameContext.GameMapContext.ResolveTurn();
                MapContainer.ClearDynamicAndPreviewGrids();
            }

            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}