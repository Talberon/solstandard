using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public class Shove : UnitSkill
    {
        public Shove() : base(
            name: "Shove",
            description: "Push a unit away one space if there is an unoccupied space behind them.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            
            if (TargetIsUnitInRange(targetSlice, targetUnit))
            {
                if (ShoveAction(targetUnit, mapContext))
                {
                    SkipCombatPhase(mapContext);
                    MapContainer.ClearDynamicGrid();
                    AssetManager.CombatBlockSFX.Play();
                }
                else
                {
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        public static bool ShoveAction(GameUnit target, MapContext mapContext)
        {
            Vector2 actorCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = target.UnitEntity.MapCoordinates;
            Vector2 oppositeCoordinates = DetermineShovePosition(actorCoordinates, targetCoordinates);

            if (TargetUnitIsInRange(target) && UnitMovingContext.CanMoveAtCoordinates(oppositeCoordinates))
            {
                target.UnitEntity.MapCoordinates = oppositeCoordinates;
                return true;
            }

            return false;
        }

        private static Vector2 DetermineShovePosition(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            Vector2 oppositeCoordinates = targetCoordinates;

            if (ActorNorthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move South
                oppositeCoordinates.Y++;
            }

            if (ActorSouthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move North
                oppositeCoordinates.Y--;
            }

            if (ActorEastOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move West
                oppositeCoordinates.X--;
            }

            if (ActorWestOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move East
                oppositeCoordinates.X++;
            }

            return oppositeCoordinates;
        }

        private static bool ActorSouthOfTarget(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            return actorCoordinates.Y > targetCoordinates.Y;
        }

        private static bool ActorWestOfTarget(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            return actorCoordinates.X < targetCoordinates.X;
        }

        private static bool ActorEastOfTarget(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            return actorCoordinates.X > targetCoordinates.X;
        }

        private static bool ActorNorthOfTarget(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            return actorCoordinates.Y < targetCoordinates.Y;
        }


        private static bool TargetUnitIsInRange(GameUnit targetUnit)
        {
            return
                targetUnit != null
                && GameContext.ActiveUnit != targetUnit
                && (MapContainer.GetMapSliceAtCoordinates(targetUnit.UnitEntity.MapCoordinates).DynamicEntity != null);
        }
    }
}