using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class MoveMapCursorAndUnitEvent : NetworkEvent
    {
        private readonly Direction direction;
        private readonly GlobalContext.GameState gameState;

        public MoveMapCursorAndUnitEvent(Direction direction, GlobalContext.GameState gameState)
        {
            this.direction = direction;
            this.gameState = gameState;
        }

        public override void Continue()
        {
            if (gameState == GlobalContext.GameState.InGame)
            {
                GlobalContext.WorldContext.MoveCursorAndSelectedUnitWithinMoveGrid(direction);
                GlobalContext.WorldContext.UpdateUnitAttackRangePreview();
            }

            Complete = true;
        }
    }
}