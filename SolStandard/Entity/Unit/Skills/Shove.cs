using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public class Shove : UnitSkill
    {
        private static readonly int[] Range = {1};

        public Shove(string name, SpriteAtlas tileSprite) : base(name, tileSprite)
        {
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
        }

        public override void ExecuteAction(GameEntity target, MapContext mapContext, BattleContext battleContext)
        {
            if (target == null || target.GetType() != typeof(GameUnit)) return;

            if (target == GameContext.ActiveUnit)
            {
                //Skip the action if selecting self
                MapContainer.ClearDynamicGrid();
                mapContext.ProceedToNextState();
                mapContext.SetPromptWindowText("Confirm End Turn");
                mapContext.ProceedToNextState();
                AssetManager.MapUnitSelectSFX.Play();
            }
            else
            {
                ShoveAction((GameUnit) target, mapContext);
            }
        }

        private static void ShoveAction(GameUnit target, MapContext mapContext)
        {
            if (!mapContext.TargetUnitIsLegal(target))
            {
                AssetManager.WarningSFX.Play();
                return;
            }

            Vector2 actorCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = target.UnitEntity.MapCoordinates;
            Vector2 oppositeCoordinates = DetermineShoveDirection(actorCoordinates, targetCoordinates);

            if (UnitMovingContext.CanMoveAtCoordinates(oppositeCoordinates))
            {
                target.UnitEntity.MapCoordinates = oppositeCoordinates;
                MapContainer.ClearDynamicGrid();
                mapContext.ProceedToNextState();
                mapContext.SetPromptWindowText("Confirm End Turn");
                mapContext.ProceedToNextState();
                AssetManager.CombatBlockSFX.Play();
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
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