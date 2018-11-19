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

    public class UnitSpriteSheet : AnimatedSpriteSheet
    {
        private UnitAnimationState currentState;

        public UnitSpriteSheet(ITexture2D spriteMap, int cellSize, Vector2 renderSize, int frameDelay, bool reversible,
            Color color, UnitAnimationState animationState = UnitAnimationState.Idle) :
            base(spriteMap, cellSize, renderSize, frameDelay, reversible, color)
        {
            SetAnimation(animationState);
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

        public override IRenderable Resize(Vector2 newSize)
        {
            return new UnitSpriteSheet(SpriteMap, CellSize, newSize, FrameDelay, Reversible, DefaultColor,
                currentState);
        }

        public new UnitSpriteSheet Clone()
        {
            return new UnitSpriteSheet(SpriteMap, CellSize, RenderSize, FrameDelay, Reversible, DefaultColor,
                currentState);
        }
    }
}