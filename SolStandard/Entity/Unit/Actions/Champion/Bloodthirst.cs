using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
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
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Bloodthirst, new Vector2(GameDriver.CellSize)),
            name: "Bloodthirst",
            description: "Perform a attack against an enemy unit and recover an " +
                         UnitStatistics.Abbreviation[Stats.Armor] + " point for each [" + damageThreshold +
                         "] damage dealt.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null
        )
        {
            this.damageThreshold = damageThreshold;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Range = GameContext.ActiveUnit.Stats.AtkRange;
            base.GenerateActionGrid(origin, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            Queue<IEvent> eventQueue = new Queue<IEvent>();
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(GameContext.ActiveUnit, new DamageToArmorStatus(Icon, 0, damageThreshold))
                );
                eventQueue.Enqueue(new StartCombatEvent(targetUnit));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}