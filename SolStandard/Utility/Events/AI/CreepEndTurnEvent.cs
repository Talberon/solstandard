using SolStandard.Containers;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.AI
{
    public class CreepEndTurnEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            MapContainer.ClearDynamicAndPreviewGrids();

            if (GlobalContext.WorldContext.SelectedUnit != null)
            {
                GlobalContext.WorldContext.SelectedUnit.SetUnitAnimation(UnitAnimationState.Idle);
            }

            //IMPORTANT Do not allow tiles that have been triggered to trigger again or the risk of soft-locking via infinite triggers can occur
            if (!WorldContext.TriggerEffectTiles(EffectTriggerTime.EndOfTurn, true))
            {
                GlobalContext.WorldContext.ResolveTurn();
                MapContainer.ClearDynamicAndPreviewGrids();
            }

            AssetManager.MapUnitCancelSFX.Play();
            Complete = true;
        }
    }
}