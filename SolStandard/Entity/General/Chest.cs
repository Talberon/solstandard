using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.General
{
    public class Chest : TerrainEntity, IActionTile, IOpenable, ILockable, ITriggerable
    {
        public enum LockIconState
        {
            Locked,
            Unlocked
        }

        private enum OpenCloseIconState
        {
            Closed,
            Open
        }

        private int Gold { get; }
        public bool IsLocked { get; private set; }
        public bool IsOpen { get; private set; }
        public int[] InteractRange { get; }
        private static readonly Color InactiveColor = new Color(50, 50, 50);

        private IItem SpecificItem { get; set; }
        public string ItemPool { get; }

        public bool CanTrigger => !IsOpen && !IsLocked;
        public bool IsObstructed => false;

        public Chest(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool isLocked, bool isOpen,
            bool canMove, int[] range, int gold, IItem item, string itemPool) :
            base(name, type, sprite, mapCoordinates)
        {
            CanMove = canMove;
            IsLocked = isLocked;
            IsOpen = isOpen;
            InteractRange = range;
            Gold = gold;

            SpecificItem = item;
            ItemPool = itemPool;
        }

        public List<UnitAction> TileActions()
        {
            var actions = new List<UnitAction>();

            if (!IsOpen) actions.Add(new OpenChestAction(this, MapCoordinates));

            return actions;
        }

        public void TakeContents()
        {
            TakeItem();
            TakeGold();
        }

        private void TakeItem()
        {
            if (SpecificItem == null)
            {
                SpecificItem = GlobalContext.WorldContext.MapContainer.GetRandomItemFromPool(ItemPool);
            }

            if (SpecificItem == null) return;
            GlobalEventQueue.QueueSingleEvent(new AddItemToUnitInventoryEvent(GlobalContext.ActiveUnit, SpecificItem));
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
        }

        private void TakeGold()
        {
            if (Gold <= 0) return;
            GlobalEventQueue.QueueSingleEvent(new IncreaseTeamGoldEvent(Gold));
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(20));
        }

        public void Trigger()
        {
            if (!CanTrigger) return;

            UnitAction toggleAction = new OpenChestAction(this, MapCoordinates, false);
            toggleAction.GenerateActionGrid(GlobalContext.ActiveUnit.UnitEntity.MapCoordinates);
            toggleAction.ExecuteAction(MapContainer.GetMapSliceAtCoordinates(MapCoordinates));
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
            MapContainer.ClearDynamicAndPreviewGrids();
        }

        public void Open()
        {
            AssetManager.DoorSFX.Play();
            ElementColor = InactiveColor;
            IsOpen = true;
        }

        public void Close()
        {
            AssetManager.DoorSFX.Play();
            ElementColor = Color.White;
            IsOpen = false;
        }

        public void ToggleLock()
        {
            AssetManager.UnlockSFX.Play();
            IsLocked = !IsLocked;
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(new[,]
                {
                    {
                        new SpriteAtlas(
                            AssetManager.LockTexture,
                            new Vector2(AssetManager.LockTexture.Width),
                            GameDriver.CellSizeVector,
                            Convert.ToInt32(
                                IsLocked ? LockIconState.Locked : LockIconState.Unlocked
                            )
                        ),
                        new RenderText(
                            AssetManager.WindowFont,
                            (IsLocked) ? "Locked" : "Unlocked",
                            (IsLocked) ? NegativeColor : PositiveColor
                        ),
                        RenderBlank.Blank
                    },
                    {
                        new SpriteAtlas(
                            AssetManager.OpenTexture,
                            new Vector2(AssetManager.OpenTexture.Width),
                            GameDriver.CellSizeVector,
                            Convert.ToInt32(
                                IsOpen ? OpenCloseIconState.Open : OpenCloseIconState.Closed
                            )
                        ),
                        new RenderText(
                            AssetManager.WindowFont,
                            (IsOpen) ? "Open" : "Closed",
                            (IsOpen) ? PositiveColor : NegativeColor
                        ),
                        RenderBlank.Blank
                    },
                    {
                        ObjectiveIconProvider.GetObjectiveIcon(
                            VictoryConditions.Taxes,
                            GameDriver.CellSizeVector
                        ),
                        new RenderText(
                            AssetManager.WindowFont,
                            ": " + (IsOpen ? Gold + Currency.CurrencyAbbreviation : "???")
                        ),
                        RenderBlank.Blank
                    },
                    {
                        string.IsNullOrEmpty(ItemPool)
                            ? RenderBlank.Blank
                            : new RenderText(AssetManager.WindowFont, "Item Pool: "),
                        string.IsNullOrEmpty(ItemPool)
                            ? RenderBlank.Blank
                            : new RenderText(AssetManager.WindowFont, ItemPool),
                        RenderBlank.Blank
                    },
                    {
                        SpecificItem != null
                            ? new RenderText(AssetManager.WindowFont, "Contents: ")
                            : RenderBlank.Blank,
                        SpecificItem?.Icon != null && IsOpen
                            ? SpecificItem.Icon.Clone()
                            : RenderBlank.Blank,
                        SpecificItem != null
                            ? new RenderText(AssetManager.WindowFont,
                                (IsOpen) ? string.Join(Environment.NewLine, SpecificItem.Name) : "????")
                            : RenderBlank.Blank
                    },
                }
            );
    }
}