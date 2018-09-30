﻿using System.Linq;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Mage
{
    public class Blink : UnitSkill
    {
        private static readonly int[] BlinkRange = {1, 2, 3, 4};

        public Blink() : base(
            name: "Blink",
            description: "Move to an unoccupied space within " + BlinkRange.Max() + "spaces.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: BlinkRange
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            if (
                UnitMovingContext.CanMoveAtCoordinates(targetSlice.MapCoordinates) && targetSlice.DynamicEntity != null
            )
            {
                GameContext.ActiveUnit.UnitEntity.MapCoordinates = targetSlice.MapCoordinates;
                MapContainer.ClearDynamicGrid();
                AssetManager.SkillBlinkSFX.Play();
                SkipCombatPhase(mapContext);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }
    }
}