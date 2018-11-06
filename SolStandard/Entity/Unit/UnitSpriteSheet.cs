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
            Color spriteColor) :
            base(spriteMap, cellSize, renderSize, frameDelay, reversible, spriteColor)
        {
            currentState = UnitAnimationState.Idle;
        }

        public Color Color
        {
            get { return SpriteColor; }
            set { SpriteColor = value; }
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

        public new UnitSpriteSheet Clone()
        {
            return new UnitSpriteSheet(SpriteMap, CellSize, RenderSize, FrameDelay, Reversible, SpriteColor);
        }
    }
}