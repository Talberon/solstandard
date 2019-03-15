using System;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;

namespace SolStandard.Utility.Events.Network
{
    public class MoveMapCursorAndUnitEvent : IEvent
    {
        private readonly Direction direction;
        private readonly GameContext.GameState gameState;

        public MoveMapCursorAndUnitEvent(Direction direction, GameContext.GameState gameState)
        {
            this.direction = direction;
            this.gameState = gameState;
        }
        public bool Complete { get; private set; }
        public void Continue()
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
                    break;
                case GameContext.GameState.MapSelect:
                    break;
                case GameContext.GameState.PauseScreen:
                    break;
                case GameContext.GameState.InGame:
                    GameContext.GameMapContext.MoveCursorAndSelectedUnitWithinMoveGrid(direction);
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