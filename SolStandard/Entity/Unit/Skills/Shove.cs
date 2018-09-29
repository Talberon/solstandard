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
        private static readonly int[] Range = {1};

        public Shove() : base(
            name: "Shove",
            description: "Push an enemy unit away one space if there is an unoccupied space behind them.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action)
        )
        {
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
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
                MapContainer.ClearDynamicGrid();
                SkipCombatPhase(mapContext);
                AssetManager.MapUnitSelectSFX.Play();
            }
            else
            {
                if (ShoveAction(targetUnit, mapContext))
                {
                    SkipCombatPhase(mapContext);
                }
            }
        }

        public static bool ShoveAction(GameUnit target, MapContext mapContext)
        {
            if (!mapContext.TargetUnitIsLegal(target))
            {
                AssetManager.WarningSFX.Play();
                return false;
            }

            Vector2 actorCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = target.UnitEntity.MapCoordinates;
            Vector2 oppositeCoordinates = DetermineShoveDirection(actorCoordinates, targetCoordinates);

            if (UnitMovingContext.CanMoveAtCoordinates(oppositeCoordinates))
            {
                target.UnitEntity.MapCoordinates = oppositeCoordinates;
                MapContainer.ClearDynamicGrid();
                AssetManager.CombatBlockSFX.Play();
                return true;
            }

            AssetManager.WarningSFX.Play();
            return false;
        }

        private static Vector2 DetermineShoveDirection(Vector2 actorCoordinates, Vector2 targetCoordinates)
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
    }
}