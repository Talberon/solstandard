using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class TrapEntity : TerrainEntity, IEffectTile, IRemotelyTriggerable, IItem
    {
        private static readonly Color InactiveColor = new Color(0, 0, 0, 50);

        private readonly bool limitedTriggers;
        private bool enabled;
        private readonly bool willSnare;
        private readonly bool willSlow;

        public int TriggersRemaining { get; private set; }
        public int Damage { get; }
        public string ItemPool { get; }
        public bool HasTriggered { get; set; }
        public bool IsExpired { get; private set; }

        public TrapEntity(string name, IRenderable sprite, Vector2 mapCoordinates, int damage, int triggersRemaining,
            bool limitedTriggers, bool enabled, bool willSnare = false, bool willSlow = false, string itemPool = null) :
            base(name, "Trap", sprite, mapCoordinates)
        {
            Damage = damage;
            TriggersRemaining = triggersRemaining;
            this.limitedTriggers = limitedTriggers;
            this.enabled = enabled;
            this.willSnare = willSnare;
            this.willSlow = willSlow;
            ItemPool = itemPool;
            IsExpired = false;
            HasTriggered = false;

            if (!enabled) Disable();
        }

        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != EffectTriggerTime.StartOfRound || HasTriggered) return false;
            if (!enabled) return false;

            MapSlice trapSlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
            GameUnit trapUnit = UnitSelector.SelectUnit(trapSlice.UnitEntity);

            if (trapUnit == null) return false;

            string trapMessage = "Trap activated!" + Environment.NewLine + trapUnit.Id + " takes [" + Damage +
                                 "] damage!";

            if (willSnare)
            {
                trapUnit.AddStatusEffect(new ImmobilizedStatus(1));
                trapMessage += Environment.NewLine + "Target is immobilized!";
            }

            if (willSlow)
            {
                trapUnit.AddStatusEffect(new MoveStatDown(2, 2));
                trapMessage += Environment.NewLine + "Target is slowed!";
            }

            for (int i = 0; i < Damage; i++)
            {
                trapUnit.DamageUnit();
            }


            TriggersRemaining--;

            if (limitedTriggers && TriggersRemaining < 1)
            {
                IsExpired = true;

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                    trapMessage + Environment.NewLine + "Trap is broken!", MapCoordinates, 80);

                AssetManager.CombatDamageSFX.Play();
                AssetManager.CombatDeathSFX.Play();
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(trapMessage, MapCoordinates,
                    80);
                AssetManager.CombatDamageSFX.Play();
            }

            GameContext.MapCursor.SnapCameraAndCursorToCoordinates(MapCoordinates);
            GameContext.MapCamera.SnapCameraCenterToCursor();
            return true;
        }

        public bool WillTrigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != EffectTriggerTime.StartOfRound || HasTriggered) return false;

            MapSlice trapSlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
            GameUnit trapUnit = UnitSelector.SelectUnit(trapSlice.UnitEntity);

            return trapUnit != null && enabled;
        }

        public void RemoteTrigger()
        {
            GameContext.MapCursor.SnapCameraAndCursorToCoordinates(MapCoordinates);
            GameContext.MapCamera.SnapCameraCenterToCursor();
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(Name + " triggered!", 50);

            ToggleTrap();
        }

        private void ToggleTrap()
        {
            if (enabled) Disable();
            else Enable();
        }

        private void Enable()
        {
            enabled = true;
            UpdateSpriteColor();
        }

        private void Disable()
        {
            enabled = false;
            UpdateSpriteColor();
        }

        private void UpdateSpriteColor()
        {
            ElementColor = (enabled) ? Color.White : InactiveColor;
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
                        new Window(new IRenderable[,]
                        {
                            {
                                UnitStatistics.GetSpriteAtlas(Stats.Atk),
                                new RenderText(AssetManager.WindowFont, "Damage: " + Damage)
                            },
                            {
                                UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                                new RenderText(AssetManager.WindowFont,
                                    (limitedTriggers) ? "Triggers Left: " + TriggersRemaining : "Permanent")
                            },
                            {
                                willSnare
                                    ? UnitStatistics.GetSpriteAtlas(Stats.Positive)
                                    : UnitStatistics.GetSpriteAtlas(Stats.Negative),
                                new RenderText(AssetManager.WindowFont, (willSnare) ? "Snares Target" : "No Snare")
                            },
                            {
                                willSlow
                                    ? UnitStatistics.GetSpriteAtlas(Stats.Positive)
                                    : UnitStatistics.GetSpriteAtlas(Stats.Negative),
                                new RenderText(AssetManager.WindowFont, (willSlow) ? "Slows Target" : "No Slow")
                            }
                        }, InnerWindowColor),
                        new RenderBlank()
                    }
                },
                1,
                HorizontalAlignment.Centered
            );

        public UnitAction UseAction()
        {
            return new LayTrap(this);
        }

        public UnitAction DropAction()
        {
            return new TradeItemAction(this);
        }

        public IItem Duplicate()
        {
            return new TrapEntity(Name, Sprite, MapCoordinates, Damage, TriggersRemaining, limitedTriggers, enabled,
                willSnare, willSlow, ItemPool);
        }

        public bool IsBroken => IsExpired;

        public IRenderable Icon => RenderSprite;
    }
}