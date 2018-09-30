﻿using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Champion
{
    public class Tackle : UnitSkill
    {
        public Tackle() : base(
            name: "Tackle",
            description: "Shove an enemy if there is an empty space behind them,"
                         + "\nthen follow up by moving into their space and attacking.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[]{1}
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (targetUnit == null || targetUnit.GetType() != typeof(GameUnit))
            {
                AssetManager.WarningSFX.Play();
                return;
            }

            if (targetUnit == GameContext.ActiveUnit)
            {
                //Skip the action if selecting self
                MapContainer.ClearDynamicGrid();
                AssetManager.MapUnitSelectSFX.Play();
                SkipCombatPhase(mapContext);
            }
            else
            {
                Vector2 targetOriginalPosition = targetUnit.UnitEntity.MapCoordinates;

                if (Shove.ShoveAction(targetUnit, mapContext))
                {
                    GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);

                    if (MoveToTarget(targetOriginalPosition))
                    {
                        GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);

                        if (BasicAttack.StartCombat(targetUnit, mapContext, battleContext))
                        {
                            EnterCombatPhase(mapContext);
                        }
                    }
                }
            }
        }

        private static bool MoveToTarget(Vector2 targetCoordinates)
        {
            if (UnitMovingContext.CanMoveAtCoordinates(targetCoordinates))
            {
                GameContext.ActiveUnit.UnitEntity.MapCoordinates = targetCoordinates;
                return true;
            }

            return false;
        }
    }
}