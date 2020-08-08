using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using SolStandard.Map;
using SolStandard.NeoGFX.Graphics;
using SolStandard.NeoGFX.GUI;
using SolStandard.NeoUtility.General;
using SolStandard.NeoUtility.Monogame.Assets;

namespace SolStandard.NeoUtility.Monogame.Interfaces
{
    public class AsepriteWrapper : IWindowContent, IPositionedRenderable
    {
        public float Width => Sprite.Size().X;
        public float Height => Sprite.Size().Y;

        public Vector2 TopLeftPoint
        {
            get => Sprite.Position;
            set => Sprite.Position = value;
        }

        public ITexture2D Texture
        {
            get => new Texture2DWrapper(Sprite.Texture);
            set => Sprite.Texture = value.MonoGameTexture;
        }

        private readonly ITexture2D spriteTexture;
        private ITexture2D? spriteTextureWhite;

        private const int BlinkingIntervalInFrames = 3;

        private readonly FrameTimedInverter<bool> flashInverter;
        private readonly FrameTimedOverride<bool> shouldBeFlashing;
        private Color flashingColor;

        protected readonly AnimatedSprite Sprite;
        private readonly Layer mapLayer;

        private bool isHidden;

        public AsepriteWrapper(AnimatedSprite sprite, Layer mapLayer = Layer.Collide,
            string? animationName = null)
        {
            Sprite = sprite;

            if (animationName is object) Sprite.Play(animationName);

            this.mapLayer = mapLayer;
            flashInverter = new FrameTimedInverter<bool>(false, true, BlinkingIntervalInFrames);
            shouldBeFlashing = new FrameTimedOverride<bool>(false);
            spriteTexture = new Texture2DWrapper(sprite.Texture);
            spriteTextureWhite = null;
            flashingColor = Color.White;
            isHidden = false;
        }

        public void FlashColorForFrames(Color flashColor, int framesToFlash, int? intervalBetweenFrames = null)
        {
            spriteTextureWhite ??= AssetManager.WhiteOutTexture(spriteTexture.MonoGameTexture);
            flashingColor = flashColor;

            flashInverter.ResetWithNewInterval(intervalBetweenFrames ?? BlinkingIntervalInFrames);
            shouldBeFlashing.OverrideForFrames(true, framesToFlash);
        }

        public void Update(GameTime gameTime)
        {
            Sprite.RenderDefinition.LayerDepth = SpriteBatchExtensions.GetLayerDepth(TopLeftPoint.Y, Height, mapLayer);
            Sprite.Update(gameTime);
            UpdateFlash();
        }

        private void UpdateFlash()
        {
            flashInverter.Update();
            shouldBeFlashing.Update();


            if (!shouldBeFlashing.Value) return;

            if (flashInverter.Value)
            {
                if (Texture == spriteTextureWhite) return;
                Texture = spriteTextureWhite;
                Sprite.RenderDefinition.Color = flashingColor;
            }
            else
            {
                if (Texture == spriteTexture) return;
                Texture = spriteTexture;
                Sprite.RenderDefinition.Color = Color.White;
            }
        }

        public void Hide()
        {
            isHidden = true;
        }

        public void Unhide()
        {
            isHidden = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isHidden) return;
            Sprite.Render(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            if (isHidden) return;
            Sprite.Position = coordinates;
            Sprite.Render(spriteBatch);
        }
    }
}