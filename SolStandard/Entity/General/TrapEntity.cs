using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class TrapEntity : TerrainEntity, IEffectTile, ITriggerable
    {
        private int triggersRemaining;
        private readonly int damage;
        private readonly bool limitedTriggers;
        private bool enabled;

        public bool IsExpired { get; private set; }

        public TrapEntity(string name, IRenderable sprite, Vector2 mapCoordinates, int damage, int triggersRemaining,
            bool limitedTriggers, bool enabled) :
            base(name, "Trap", sprite, mapCoordinates, new Dictionary<string, string>())
        {
            this.damage = damage;
            this.triggersRemaining = triggersRemaining;
            this.limitedTriggers = limitedTriggers;
            this.enabled = enabled;
            IsExpired = false;
        }

        public void TriggerStartOfTurn()
        {
            if (!enabled) return;

            MapSlice trapSlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
            GameUnit trapUnit = UnitSelector.SelectUnit(trapSlice.UnitEntity);

            if (trapUnit == null || trapUnit != GameContext.ActiveUnit) return;

            for (int i = 0; i < damage; i++)
            {
                trapUnit.DamageUnit();
            }

            string trapMessage = "Trap activated!" + Environment.NewLine +
                                 trapUnit.Id + " takes [" + damage + "] damage!";


            AssetManager.CombatDamageSFX.Play();

            if (!limitedTriggers)
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(trapMessage, MapCoordinates,
                    50);
                return;
            }

            triggersRemaining--;

            if (triggersRemaining < 1)
            {
                IsExpired = true;

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                    trapMessage + Environment.NewLine + "Trap is broken!", MapCoordinates, 50);

                AssetManager.CombatDeathSFX.Play();
            }
        }

        public void TriggerEndOfTurn()
        {
            //Do nothing
        }

        public void Trigger()
        {
            enabled = !enabled;
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
                            UnitStatistics.GetSpriteAtlas(Stats.Atk),
                            new RenderText(AssetManager.WindowFont, "Damage: " + damage)
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                            new RenderText(AssetManager.WindowFont, "Triggers Left: " + triggersRemaining)
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        }
                    },
                    1
                );
            }
        }
    }
}