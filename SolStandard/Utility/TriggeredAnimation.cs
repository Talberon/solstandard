using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility
{
    public class TriggeredAnimation : AnimatedSpriteSheet
    {
        private bool isVisible;

        public TriggeredAnimation(ITexture2D spriteMap, int cellSize, Vector2 renderSize, int frameDelay,
            Color color, bool reversible = false, bool isFlipped = false) :
            base(spriteMap, cellSize, renderSize, frameDelay, reversible, color, isFlipped)
        {
            isVisible = false;
        }

        public TriggeredAnimation(AnimatedSpriteSheet sourceSheet)
            : this(sourceSheet.SpriteMap, sourceSheet.CellSize, sourceSheet.RenderSize, sourceSheet.FrameDelay,
                sourceSheet.DefaultColor, sourceSheet.Reversible, sourceSheet.IsFlipped)
        {
        }

        public new void PlayOnce()
        {
            isVisible = true;
            ResetAnimation();
        }

        private void CheckComplete()
        {
            if ((CurrentColumn == SpriteFrameCount - 1) && (FrameDelayCounter % FrameDelay == 0))
            {
                isVisible = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            if (isVisible) base.Draw(spriteBatch, position, colorOverride);

            CheckComplete();
        }
    }
}