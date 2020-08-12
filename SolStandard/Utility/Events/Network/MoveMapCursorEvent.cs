using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class MoveMapCursorEvent : NetworkEvent
    {
        private readonly Direction direction;
        private readonly GlobalContext.GameState gameState;

        public MoveMapCursorEvent(Direction direction, GlobalContext.GameState gameState)
        {
            this.direction = direction;
            this.gameState = gameState;
        }

        public override void Continue()
        {
            switch (gameState)
            {
                case GlobalContext.GameState.Deployment:
                    GlobalContext.DeploymentContext.MoveCursorOnMap(direction);
                    break;
                case GlobalContext.GameState.MapSelect:
                    GlobalContext.MapCursor.MoveCursorInDirection(direction);
                    GlobalContext.MapSelectContext.HoverOverEntity();
                    break;
                case GlobalContext.GameState.InGame:
                    GlobalContext.WorldContext.MoveCursorOnMap(direction);
                    break;
            }

            Complete = true;
        }
    }
}