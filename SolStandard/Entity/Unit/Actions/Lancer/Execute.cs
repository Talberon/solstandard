using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class Execute : UnitAction
    {
        private readonly int damageToDeal;

        public Execute(int damageToDeal) : base(
            //TODO Add Execute Skill Icon
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, new Vector2(GameDriver.CellSize)),
            name: "Execute",
            description: "Deal a finishing blow to target. If target dies, regenerate all " +
                         UnitStatistics.Abbreviation[Stats.Armor] + " and gain an " +
                         UnitStatistics.Abbreviation[Stats.Atk] + " Up buff.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
            this.damageToDeal = damageToDeal;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Range = GameContext.ActiveUnit.Stats.CurrentAtkRange;
            base.GenerateActionGrid(origin, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            WeaponStatistics executionersKnife =
                new WeaponStatistics(damageToDeal, 0, GameContext.ActiveUnit.Stats.CurrentAtkRange, 1);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(GameContext.ActiveUnit, new ExecutionerStatus(Icon, 0)));
                eventQueue.Enqueue(
                    new StartCombatEvent(
                        targetUnit,
                        GameContext.ActiveUnit.Stats.ApplyWeaponStatistics(executionersKnife)
                    )
                );
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