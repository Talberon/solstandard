using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.AI
{
    public class CreepMoveEvent : IEvent
    {
        private readonly GameUnit unitToMove;
        private readonly Direction directionToMove;
        private readonly bool ignoreCollision;

        public CreepMoveEvent(GameUnit unitToMove, Direction directionToMove, bool ignoreCollision = false)
        {
            this.unitToMove = unitToMove;
            this.directionToMove = directionToMove;
            this.ignoreCollision = ignoreCollision;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            unitToMove.MoveUnitInDirection(directionToMove, ignoreCollision);
            GameContext.GameMapContext.ResetCursorToActiveUnit();

            if (directionToMove != Direction.None) AssetManager.MapUnitMoveSFX.Play();

            Complete = true;
        }
    }
}