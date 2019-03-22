using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class TrapEntity : TerrainEntity, IEffectTile, IRemotelyTriggerable
    {
        private static readonly Color InactiveColor = new Color(0, 0, 0, 50);

        private int triggersRemaining;
        private readonly int damage;
        private readonly bool limitedTriggers;
        private bool enabled;
        private readonly bool willSnare;

        public bool IsExpired { get; private set; }

        public TrapEntity(string name, IRenderable sprite, Vector2 mapCoordinates, int damage, int triggersRemaining,
            bool limitedTriggers, bool enabled, bool willSnare = false) :
            base(name, "Trap", sprite, mapCoordinates, new Dictionary<string, string>())
        {
            this.damage = damage;
            this.triggersRemaining = triggersRemaining;
            this.limitedTriggers = limitedTriggers;
            this.enabled = enabled;
            this.willSnare = willSnare;
            IsExpired = false;
        }

        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != EffectTriggerTime.StartOfTurn) return false;

            if (!enabled) return false;

            MapSlice trapSlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
            GameUnit trapUnit = UnitSelector.SelectUnit(trapSlice.UnitEntity);

            if (trapUnit == null) return false;

            string trapMessage = "Trap activated!" + Environment.NewLine + trapUnit.Id + " takes [" + damage +
                                 "] damage!";

            if (willSnare)
            {
                trapUnit.AddStatusEffect(new ImmobilizedStatus(1));
                trapMessage += Environment.NewLine + "Target is immobilized!";
            }

            for (int i = 0; i < damage; i++)
            {
                trapUnit.DamageUnit();
            }


            triggersRemaining--;

            if (limitedTriggers && triggersRemaining < 1)
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
                                    new RenderText(AssetManager.WindowFont, "Damage: " + damage)
                                },
                                {
                                    UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                                    new RenderText(AssetManager.WindowFont,
                                        (limitedTriggers) ? "Triggers Left: " + triggersRemaining : "Permanent")
                                }
                            }, InnerWindowColor),
                            new RenderBlank()
                        }
                    },
                    1
                );
            }
        }
    }
}