using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Rogue
{
    public class ThrowingKnife : UnitAction
    {
        public ThrowingKnife() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.ThrowingKnife, GameDriver.CellSizeVector),
            name: "Throwing Knife",
            description: "Perform a basic ranged attack. " + Environment.NewLine +
                         "Will not affect range when other units attack this one.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {2},
            freeAction: false
        )
        {
        }


        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GlobalEventQueue.QueueSingleEvent(new StartCombatEvent(targetUnit));
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}