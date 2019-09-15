using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.General
{
    public class Chest : TerrainEntity, IActionTile, IOpenable, ILockable, ITriggerable
    {
        private enum LockIconState
        {
            Locked,
            Unlocked
        }

        private enum OpenCloseIconState
        {
            Closed,
            Open
        }

        public int Gold { get; }
        public bool IsLocked { get; private set; }
        public bool IsOpen { get; private set; }
        public int[] InteractRange { get; }
        private static readonly Color InactiveColor = new Color(50, 50, 50);

        public List<IItem> Items { get; }

        public Chest(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool isLocked, bool isOpen,
            bool canMove, int[] range,
            int gold, IItem item = null) :
            base(name, type, sprite, mapCoordinates)
        {
            CanMove = canMove;
            IsLocked = isLocked;
            IsOpen = isOpen;
            InteractRange = range;
            Gold = gold;

            Items = new List<IItem>();
            if (item != null) Items.Add(item);
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                List<string> itemList = new List<string>();
                Items.ForEach(item => itemList.Add(item.Name));

                return new WindowContentGrid(
                    new[,]
                    {
                        {base.TerrainInfo},
                        {
                            new Window(new WindowContentGrid(new IRenderable[,]
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
                                        new RenderText(AssetManager.WindowFont, (IsLocked) ? "Locked" : "Unlocked",
                                            (IsLocked) ? NegativeColor : PositiveColor)
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
                                        new RenderText(AssetManager.WindowFont, (IsOpen) ? "Open" : "Closed",
                                            (IsOpen) ? PositiveColor : NegativeColor)
                                    },
                                    {
                                        ObjectiveIconProvider.GetObjectiveIcon(
                                            VictoryConditions.Taxes,
                                            GameDriver.CellSizeVector
                                        ),
                                        new RenderText(AssetManager.WindowFont,
                                            ": " + (IsOpen ? Gold + Currency.CurrencyAbbreviation : "???"))
                                    },
                                    {
                                        new RenderText(AssetManager.WindowFont, "Contents: "),
                                        new RenderText(AssetManager.WindowFont,
                                            (IsOpen) ? string.Join(Environment.NewLine, itemList) : "????"),
                                    },
                                }, 1),
                                InnerWindowColor),
                        }
                    },
                    3,
                    HorizontalAlignment.Centered
                );
            }
        }

        public List<UnitAction> TileActions()
        {
            List<UnitAction> actions = new List<UnitAction>();

            if (!IsOpen) actions.Add(new OpenChestAction(this, MapCoordinates));

            return actions;
        }


        public void Trigger()
        {
            if (!CanTrigger) return;

            UnitAction toggleAction = new OpenChestAction(this, MapCoordinates, false);
            toggleAction.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
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

        public bool CanTrigger => !IsOpen && !IsLocked;

        public bool IsObstructed => false;
    }
}