using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
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

        public int TriggersRemaining { get; private set; }
        public int Damage { get; private set; }
        public string ItemPool { get; private set; }
        public bool IsExpired { get; private set; }

        public TrapEntity(string name, IRenderable sprite, Vector2 mapCoordinates, int damage, int triggersRemaining,
            bool limitedTriggers, bool enabled, bool willSnare = false, string itemPool = null) :
            base(name, "Trap", sprite, mapCoordinates, new Dictionary<string, string>())
        {
            Damage = damage;
            TriggersRemaining = triggersRemaining;
            this.limitedTriggers = limitedTriggers;
            this.enabled = enabled;
            this.willSnare = willSnare;
            ItemPool = itemPool;
            IsExpired = false;
        }

        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != EffectTriggerTime.StartOfTurn) return false;

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

            for (int i = 0; i < Damage; i++)
            {
                trapUnit.DamageUnit();
            }


            TriggersRemaining--;

            if (limitedTriggers && TriggersRemaining < 1)
            {
                IsExpired = true;

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                    trapMessage + Environment.NewLine + "Trap is broken!", MapCoordinates, 50);

                AssetManager.CombatDamageSFX.Play();
                AssetManager.CombatDeathSFX.Play();
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(trapMessage, MapCoordinates,
                    50);
                AssetManager.CombatDamageSFX.Play();
            }

            GameContext.MapCursor.SnapCursorToCoordinates(MapCoordinates);
            GameContext.MapCamera.SnapCameraCenterToCursor();
            return true;
        }

        public void RemoteTrigger()
        {
            enabled = !enabled;

            ElementColor = (enabled) ? Color.White : InactiveColor;
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
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
                                }
                            }, InnerWindowColor),
                            new RenderBlank()
                        }
                    },
                    1
                );
            }
        }

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
                willSnare, ItemPool);
        }

        public bool IsBroken
        {
            get { return IsExpired; }
        }

        public IRenderable Icon
        {
            get { return RenderSprite; }
        }
    }
}