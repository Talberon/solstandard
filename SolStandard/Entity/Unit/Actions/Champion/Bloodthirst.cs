using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Champion
{
    public class Bloodthirst : UnitAction
    {
        private readonly int damageThreshold;

        public Bloodthirst(int damageThreshold) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Bloodthirst, GameDriver.CellSizeVector),
            name: $"Bloodthirst [{damageThreshold}]",
            description:
            $"Attack an enemy unit and recover an {UnitStatistics.Abbreviation[Stats.Armor]} point for every [{damageThreshold}] damage dealt.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null,
            freeAction: false
        )
        {
            this.damageThreshold = damageThreshold;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Range = GlobalContext.ActiveUnit.Stats.CurrentAtkRange;
            base.GenerateActionGrid(origin, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(GlobalContext.ActiveUnit, new DamageToArmorStatus(Icon, damageThreshold))
                );
                eventQueue.Enqueue(new StartCombatEvent(targetUnit));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}