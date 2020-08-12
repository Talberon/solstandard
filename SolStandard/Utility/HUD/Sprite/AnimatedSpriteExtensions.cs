using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;

namespace SolStandard.Utility.HUD.Sprite
{
    public static class AnimatedSpriteExtensions
    {
        public static void PlayOnceThenFreeze(this AnimatedSprite me, string animationName)
        {
            me.OnAnimationLoop = () =>
            {
                me.Animating = false;
                me.CurrentFrameIndex = me.CurrentAnimation.to;
                me.OnAnimationLoop = null;
            };
            me.Play(animationName);
        }

        public static void PlayOnceThenPlayAnother(this AnimatedSprite me, string transitionAnimation,
            string targetAnimation)
        {
            me.OnAnimationLoop = () =>
            {
                me.Play(targetAnimation);
                me.OnAnimationLoop = null;
            };
            me.Play(transitionAnimation);
        }

        public static void Resize(this AnimatedSprite me, float sizePx)
        {
            me.RenderDefinition.Scale = new Vector2(sizePx / me.FrameSize().Y);
        }

        public static void FlipHorizontal(this AnimatedSprite me)
        {
            me.RenderDefinition.SpriteEffect = (me.RenderDefinition.SpriteEffect == SpriteEffects.FlipHorizontally)
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;
        }

        public static AnimatedSprite WithAnimation(this AnimatedSprite me, string animationName)
        {
            me.Play(animationName);
            return me;
        }

        public static AnimatedSprite WithSize(this AnimatedSprite me, float sizePx)
        {
            me.Resize(sizePx);
            return me;
        }
        

        public static AsepriteWrapper ToWrapper(this AnimatedSprite me)
        {
            return new AsepriteWrapper(me);
        }
        
        public static Vector2 Size(this AnimatedSprite me)
        {
            return new Vector2(
                me.CurrentFrame.frame.Width * me.RenderDefinition.Scale.X,
                me.CurrentFrame.frame.Height * me.RenderDefinition.Scale.Y
            );
        }

        public static Vector2 FrameSize(this AnimatedSprite me)
        {
            return new Vector2(me.CurrentFrame.frame.Width, me.CurrentFrame.frame.Height);
        }
    }
}