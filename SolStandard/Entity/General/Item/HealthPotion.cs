﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.General.Item
{
    public class HealthPotion : TerrainEntity, IActionTile, IConsumable
    {
        private static readonly int[] UseRange = {0, 1};

        public int[] InteractRange { get; }
        private int HPHealed { get; }
        public bool IsBroken { get; set; }
        public string ItemPool { get; }

        public HealthPotion(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            int hpHealed, string itemPool)
            : base(name, type, sprite, mapCoordinates)
        {
            InteractRange = pickupRange;
            HPHealed = hpHealed;
            ItemPool = itemPool;
        }

        public IRenderable Icon => Sprite;

        public void Consume(GameUnit targetUnit)
        {
            IsBroken = true;
            GameContext.ActiveUnit.RemoveItemFromInventory(this);
            GlobalEventQueue.QueueSingleEvent(new RegenerateHealthEvent(targetUnit, HPHealed));
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PickUpItemAction(this, MapCoordinates)
            };
        }

        public UnitAction UseAction()
        {
            return new ConsumeRecoveryItemAction(this, HPHealed, UseRange);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new HealthPotion(Name, Type, Sprite, MapCoordinates, InteractRange, HPHealed, ItemPool);
        }

        public override IRenderable TerrainInfo =>
            new WindowContentGrid(
                new[,]
                {
                    {
                        InfoHeader,
                        new RenderBlank()
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Mv),
                        new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                            (CanMove) ? PositiveColor : NegativeColor)
                    },
                    {
                        StatusIconProvider.GetStatusIcon(StatusIcon.PickupRange, new Vector2(GameDriver.CellSize)),
                        new RenderText(
                            AssetManager.WindowFont,
                            ": " + $"[{string.Join(",", InteractRange)}]"
                        )
                    },
                    {
                        new Window(new IRenderable[,]
                        {
                            {
                                UnitStatistics.GetSpriteAtlas(Stats.Hp, new Vector2(GameDriver.CellSize)),
                                new RenderText(AssetManager.WindowFont, "Heal : +" + HPHealed + "")
                            }
                        }, InnerWindowColor),
                        new RenderBlank()
                    }
                },
                3
            );
    }
}