using System;
using SolStandard.Containers.Contexts;
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
                    GameContext.GameMapContext.UpdateUnitAttackRangePreview();
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