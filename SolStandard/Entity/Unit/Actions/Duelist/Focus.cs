using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses.Duelist;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Duelist
{
    public class Focus : UnitAction
    {
        private readonly int maxActions;

        public Focus(int maxActions) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Focus, GameDriver.CellSizeVector),
            name: "Focus",
            description: "End your action now and store it for later. Can store up to " + maxActions +
                         " action(s) at a time.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: false
        )
        {
            this.maxActions = maxActions;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                if (targetUnit.StatusEffects.SingleOrDefault(status => status is FocusStatus) is FocusStatus
                    currentFocus)
                {
                    if (currentFocus.FocusPoints < maxActions)
                    {
                        AssetManager.SkillBuffSFX.Play();
                        AssetManager.MenuConfirmSFX.Play();
                        GlobalEventQueue.QueueSingleEvent(
                            new CastStatusEffectEvent(targetUnit, new FocusStatus(currentFocus.FocusPoints + 1, false))
                        );
                        GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
                    }
                    else
                    {
                        GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Max focus points reached!", 50);
                        AssetManager.WarningSFX.Play();
                    }
                }
                else
                {
                    GlobalEventQueue.QueueSingleEvent(new CastStatusEffectEvent(targetUnit, new FocusStatus(1, false)));
                    GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}