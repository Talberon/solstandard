using Microsoft.Xna.Framework;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public class UnitSprite : AnimatedSprite
    {
        public enum UnitAnimationState
        {
            Idle,
            Attack,
            WalkLeft,
            WalkRight,
            WalkDown,
            WalkUp,
            WalkSW,
            WalkSE,
            WalkNW,
            WalkNE
        }

        private UnitAnimationState currentState;

        public UnitSprite(ITexture2D spriteMap, int cellSize, int frameDelay, bool reversible) : this(spriteMap,
            cellSize, new Vector2(cellSize),  frameDelay, reversible)
        {
        }
        
        public UnitSprite(ITexture2D spriteMap, int cellSize, Vector2 renderSize, int frameDelay, bool reversible) : base(spriteMap,
            cellSize, renderSize, frameDelay, reversible)
        {
            currentState = UnitAnimationState.Idle;
        }

        public void SetAnimation(UnitAnimationState state)
        {
            currentState = state;
            SetSpriteCell(0, (int) currentState);
        }
    }
}