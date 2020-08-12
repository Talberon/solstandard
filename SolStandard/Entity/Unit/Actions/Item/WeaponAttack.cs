using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
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
        private readonly Weapon sourceWeapon;

        public WeaponAttack(IRenderable icon, WeaponStatistics stats, Weapon sourceWeapon) : base(
            icon: icon,
            name: "Attack",
            description: WeaponDescription(stats),
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null,
            freeAction: false
        )
        {
            this.stats = stats;
            this.sourceWeapon = sourceWeapon;
        }

        private static WindowContentGrid WeaponDescription(WeaponStatistics stats)
        {
            return new WindowContentGrid(new IRenderable[,]
                {
                    {
                        new RenderText(
                            AssetManager.WindowFont,
                            "Perform a basic attack against target based on your weapon's statistics." +
                            Environment.NewLine + "Can also be used against certain terrain."
                        )
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
            var eventQueue = new Queue<IEvent>();
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (!stats.IsBroken)
            {
                if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
                {
                    UseWeapon();

                    eventQueue.Enqueue(
                        new StartCombatEvent(targetUnit, false,
                            GlobalContext.ActiveUnit.Stats.ApplyWeaponStatistics(stats))
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
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Weapon is broken!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private void UseWeapon()
        {
            stats.DecrementRemainingUses();
            if (!stats.IsBroken) return;

            GlobalContext.ActiveUnit.RemoveItemFromInventory(sourceWeapon);
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Weapon is broken!", 50);
            AssetManager.CombatDeathSFX.Play();

            if (Icon is SpriteAtlas icon) icon.DefaultColor = GameUnit.DeadPortraitColor;
        }
    }
}