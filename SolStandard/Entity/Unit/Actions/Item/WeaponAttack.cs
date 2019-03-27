using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.HUD.Window.Content;
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
            description:  WeaponDescription(stats),
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null
        )
        {
            this.stats = stats;
        }

        private static WindowContentGrid WeaponDescription(WeaponStatistics stats)
        {
            return new WindowContentGrid(new IRenderable[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont, "Perform a basic attack against target based on your weapon's statistics."), 
                    },
                    {
                        stats.GenerateStatGrid(AssetManager.WindowFont)
                    }
                },
                2
            );
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
                    UseWeapon();

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

        private void UseWeapon()
        {
            stats.DecrementRemainingUses();
            if (!stats.IsBroken) return;

            GameContext.ActiveUnit.InventoryActions.Remove(this);

            SpriteAtlas icon = Icon as SpriteAtlas;
            if (icon != null) icon.DefaultColor = GameUnit.DeadPortraitColor;
        }
    }
}