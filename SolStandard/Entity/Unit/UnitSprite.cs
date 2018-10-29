using Microsoft.Xna.Framework;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
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

    public class UnitSprite : AnimatedSprite
    {
        private UnitAnimationState currentState;

        public UnitSprite(ITexture2D spriteMap, int cellSize, Vector2 renderSize, int frameDelay, bool reversible) :
            base(spriteMap, cellSize, renderSize, frameDelay, reversible)
        {
            currentState = UnitAnimationState.Idle;
        }

        public void SetFrameDelay(int frameDelay)
        {
            FrameDelay = frameDelay;
        }

        public void ResetFrameDelay()
        {
            FrameDelay = DefaultFrameDelay;
        }

        public void SetAnimation(UnitAnimationState state)
        {
            currentState = state;
            SetSpriteCell(0, (int) currentState);
        }
        
        public new UnitSprite Clone()
        {
            return new UnitSprite(SpriteMap, CellSize, RenderSize, FrameDelay, Reversible);
        }
    }
}