using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class WeaponAttack : UnitAction
    {
        private readonly WeaponStatistics stats;

        public WeaponAttack(IRenderable icon, string weaponName, WeaponStatistics stats) : base(
            icon: icon,
            name: "Attack: " + weaponName,
            description: "Attack a target with dice based on your weapon's statistics." + Environment.NewLine + stats,
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
            this.stats = stats;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Range = stats.AtkRange;
            base.GenerateActionGrid(origin, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            Queue<IEvent> eventQueue = new Queue<IEvent>();
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (!stats.IsBroken)
            {
                if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
                {
                    stats.DecrementRemainingUses();
                    eventQueue.Enqueue(
                        new StartCombatEvent(targetUnit, GameContext.ActiveUnit.Stats.ApplyWeaponStatistics(stats))
                    );
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else if (TargetIsABreakableObstacleInRange(targetSlice))
                {
                    stats.DecrementRemainingUses();
                    BasicAttack.DamageTerrain(targetSlice);
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new EndTurnEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Weapon is broken!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}