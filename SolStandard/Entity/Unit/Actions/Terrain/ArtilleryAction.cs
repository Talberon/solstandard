using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General.Item;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class ArtilleryAction : UnitAction
    {
        private readonly WeaponStatistics weaponStatistics;

        public ArtilleryAction(IRenderable tileIcon, int[] range, WeaponStatistics weaponStatistics) : base(
            icon: tileIcon,
            name: "Artillery",
            description: "Attack a target at an extended range based on the range of this weapon.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: range,
            freeAction: false
        )
        {
            this.weaponStatistics = weaponStatistics;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GlobalEventQueue.QueueSingleEvent(
                    new StartCombatEvent(
                        targetUnit,
                        false, 
                        GlobalContext.ActiveUnit.Stats.ApplyWeaponStatistics(weaponStatistics)
                    )
                );
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a valid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}