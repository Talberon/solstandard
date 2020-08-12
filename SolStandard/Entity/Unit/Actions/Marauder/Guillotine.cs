using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses.Marauder;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Marauder
{
    public class Guillotine : UnitAction
    {
        private readonly int healPercentage;

        public Guillotine(int healPercentage) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Guillotine, GameDriver.CellSizeVector),
            name: "Guillotine",
            description:
            $"Perform a basic attack. If the target unit is defeated, regenerate {healPercentage}% of missing " +
            $"{UnitStatistics.Abbreviation[Stats.Hp]} (rounded up).",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null,
            freeAction: false
        )
        {
            this.healPercentage = healPercentage;
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
                    new CastStatusEffectEvent(GlobalContext.ActiveUnit, new GuillotineStatus(Icon, 0, healPercentage))
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