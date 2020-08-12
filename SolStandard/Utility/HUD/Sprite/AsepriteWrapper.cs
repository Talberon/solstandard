#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using SolStandard.Utility.Monogame;
using NotImplementedException = System.NotImplementedException;

namespace SolStandard.Utility.HUD.Sprite
{
    public class AsepriteWrapper : IRenderable
    {
        public Color DefaultColor { get; set; } //Ignore this
        public int Width => (int) Sprite.Size().X;
        public int Height => (int) Sprite.Size().Y;

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

        protected readonly AnimatedSprite Sprite;

        private bool isHidden;

        public AsepriteWrapper(AnimatedSprite sprite, string? animationName = null)
        {
            Sprite = sprite;

            if (animationName is object) Sprite.Play(animationName);

            isHidden = false;
        }

        public void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
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

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            throw new NotImplementedException();
        }

        public IRenderable Clone()
        {
            throw new NotImplementedException();
        }
    }
}