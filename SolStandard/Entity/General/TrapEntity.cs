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
    public class TrapEntity : TerrainEntity, IEffectTile
    {
        private int triggersRemaining;
        private readonly int damage;

        public bool IsExpired { get; private set; }

        public TrapEntity(string name, IRenderable sprite, Vector2 mapCoordinates, int damage, int triggersRemaining) :
            base(name, "Trap", sprite, mapCoordinates, new Dictionary<string, string>())
        {
            this.damage = damage;
            this.triggersRemaining = triggersRemaining;
            IsExpired = false;
        }

        public void TriggerStartOfTurn()
        {
            MapSlice trapSlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
            GameUnit trapUnit = UnitSelector.SelectUnit(trapSlice.UnitEntity);

            if (trapUnit == null || trapUnit != GameContext.ActiveUnit) return;

            for (int i = 0; i < damage; i++)
            {
                trapUnit.DamageUnit();
            }

            string trapMessage = "Trap activated!" + Environment.NewLine +
                                 trapUnit.Id + " takes [" + damage + "] damage!";

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(trapMessage, MapCoordinates, 50);

            AssetManager.CombatDamageSFX.Play();

            triggersRemaining--;

            if (triggersRemaining < 1)
            {
                IsExpired = true;

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates("Trap is broken!",
                    MapCoordinates, 50);

                AssetManager.CombatDeathSFX.Play();
            }
        }

        public void TriggerEndOfTurn()
        {
            //Do nothing
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
                            Sprite,
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