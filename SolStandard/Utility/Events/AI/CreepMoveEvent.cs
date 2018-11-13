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

        public CreepMoveEvent(GameUnit unitToMove, Direction directionToMove)
        {
            this.unitToMove = unitToMove;
            this.directionToMove = directionToMove;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            unitToMove.MoveUnitInDirection(directionToMove);
            GameContext.GameMapContext.ResetCursorToActiveUnit();
            AssetManager.MapUnitMoveSFX.Play();
            Complete = true;
        }
    }
}