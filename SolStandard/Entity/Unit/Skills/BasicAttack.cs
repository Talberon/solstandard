﻿using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public class BasicAttack : UnitSkill
    {
        private readonly int[] range;

        public BasicAttack(string name, SpriteAtlas tileSprite, int[] range) : base(name, tileSprite)
        {
            this.range = range;
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, range);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (targetUnit != null && targetUnit.GetType() == typeof(GameUnit))
            {
                //Skip the combat state if player selects the same unit
                if (mapContext.SelectedUnit == targetUnit)
                {
                    MapContainer.ClearDynamicGrid();
                    mapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                    SkipCombatPhase(mapContext);
                    AssetManager.MapUnitSelectSFX.Play();
                }
                else
                {
                    if (StartCombat(targetUnit, mapContext, battleContext))
                    {
                        EnterCombatPhase(mapContext);
                    }
                }
            }
        }

        public static bool StartCombat(GameUnit target, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit attackingUnit = mapContext.SelectedUnit;
            GameUnit defendingUnit = target;

            if (mapContext.TargetUnitIsLegal(defendingUnit))
            {
                MapContainer.ClearDynamicGrid();
                battleContext.StartNewCombat(attackingUnit,
                    MapContainer.GetMapSliceAtCoordinates(attackingUnit.UnitEntity.MapCoordinates),
                    defendingUnit,
                    MapContainer.GetMapSliceAtCoordinates(defendingUnit.UnitEntity.MapCoordinates));

                AssetManager.CombatStartSFX.Play();
                return true;
            }

            AssetManager.WarningSFX.Play();
            return false;
        }
    }
}