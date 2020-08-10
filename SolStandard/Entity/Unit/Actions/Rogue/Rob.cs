using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Rogue
{
    public class Rob : UnitAction
    {
        public Rob() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Rob, GameDriver.CellSizeVector),
            name: "Rob",
            description: "Take one item that an enemy is holding." + Environment.NewLine +
                         $"Will not work if target has any {UnitStatistics.Abbreviation[Stats.Armor]} remaining.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                if (targetUnit.Stats.CurrentArmor == 0)
                {
                    if (targetUnit.Inventory.Count > 0)
                    {
                        MapContainer.ClearDynamicAndPreviewGrids();
                        GlobalContext.WorldContext.OpenTakeItemMenu(targetUnit, false);
                    }
                    else
                    {
                        GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                            "Target has no items in inventory!", 50
                        );
                        AssetManager.WarningSFX.Play();
                    }
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                        $"Can't target unit with {UnitStatistics.Abbreviation[Stats.Armor]} remaining!", 50
                    );
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

    }
}