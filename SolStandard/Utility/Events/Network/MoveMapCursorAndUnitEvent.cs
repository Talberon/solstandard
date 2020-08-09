using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class MoveMapCursorAndUnitEvent : NetworkEvent
    {
        private readonly Direction direction;
        private readonly GameContext.GameState gameState;

        public MoveMapCursorAndUnitEvent(Direction direction, GameContext.GameState gameState)
        {
            this.direction = direction;
            this.gameState = gameState;
        }

        public override void Continue()
        {
            if (gameState == GameContext.GameState.InGame)
            {
                GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(direction);
                GameContext.GameMapContext.UpdateUnitAttackRangePreview();
            }

            Complete = true;
        }
    }
}