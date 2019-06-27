using System;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class MoveMapCursorEvent : NetworkEvent
    {
        private readonly Direction direction;
        private readonly GameContext.GameState gameState;

        public MoveMapCursorEvent(Direction direction, GameContext.GameState gameState)
        {
            this.direction = direction;
            this.gameState = gameState;
        }
        
        public override void Continue()
        {
            switch (gameState)
            {
                case GameContext.GameState.MainMenu:
                    break;
                case GameContext.GameState.NetworkMenu:
                    break;
                case GameContext.GameState.ArmyDraft:
                    break;
                case GameContext.GameState.Deployment:
                    GameContext.DeploymentContext.MoveCursorOnMap(direction);
                    break;
                case GameContext.GameState.MapSelect:
                    GameContext.MapCursor.MoveCursorInDirection(direction);
                    GameContext.MapSelectContext.HoverOverEntity();
                    break;
                case GameContext.GameState.PauseScreen:
                    break;
                case GameContext.GameState.InGame:
                    GameContext.GameMapContext.MoveCursorOnMap(direction);
                    break;
                case GameContext.GameState.Results:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Complete = true;
        }
    }
}