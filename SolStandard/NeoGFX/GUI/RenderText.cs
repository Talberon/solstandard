using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Map;
using SolStandard.NeoUtility.Monogame.Interfaces;

namespace SolStandard.NeoGFX.GUI
{
    public class RenderText : IWindowContent
    {
        private readonly ISpriteFont font;
        private readonly bool hasOutline;
        private readonly Color? outlineColor;
        public string Message { get; set; }
        public Color DefaultColor { get; set; }
        public Vector2 Position { get; set; }
        public float Height => font.MeasureString(Message).Y;
        public float Width => font.MeasureString(Message).X;

        public RenderText(ISpriteFont font, string message, Color color, Vector2 position, bool hasOutline = false,
            Color? outlineColor = null)
        {
            this.font = font;
            this.hasOutline = hasOutline;
            this.outlineColor = outlineColor ?? Color.Black;
            Message = message;
            DefaultColor = color;
            Position = position;
        }

        public RenderText(ISpriteFont font, string message, Color color, bool hasOutline = false,
            Color? outlineColor = null) :
            this(font, message, color, Vector2.Zero, hasOutline: hasOutline, outlineColor: outlineColor)
        {
            //Intentionally left blank
        }

        public RenderText(ISpriteFont font, string message, bool hasOutline = false, Color? textColor = null) :
            this(font, message, textColor ?? Color.White, Vector2.Zero, hasOutline: hasOutline,
                outlineColor: Color.Black)
        {
            //Intentionally left blank
        }

        public void Update(GameTime gameTime)
        {
            //Do nothing.
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Position);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            if (hasOutline && outlineColor is object)
            {
                DrawOutline(spriteBatch, coordinates);
            }

            spriteBatch.DrawString(
                spriteFont: font.MonoGameSpriteFont,
                text: Message,
                position: coordinates,
                color: DefaultColor,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 1f
            );
        }

        private void DrawOutline(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            if (outlineColor is null) throw new NoNullAllowedException("OutlineColor cannot be null!");

            spriteBatch.DrawString(
                spriteFont: font.MonoGameSpriteFont,
                text: Message,
                position: coordinates + Vector2.UnitX,
                color: outlineColor.Value,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 1f / (coordinates.Y * (int) Layer.OverlayEffect - 1)
            );

            spriteBatch.DrawString(
                spriteFont: font.MonoGameSpriteFont,
                text: Message,
                position: coordinates - Vector2.UnitX,
                color: outlineColor.Value,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 1f / (coordinates.Y * (int) Layer.OverlayEffect - 1)
            );

            spriteBatch.DrawString(
                spriteFont: font.MonoGameSpriteFont,
                text: Message,
                position: coordinates + Vector2.UnitY,
                color: outlineColor.Value,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 1f / (coordinates.Y * (int) Layer.OverlayEffect - 1)
            );

            spriteBatch.DrawString(
                spriteFont: font.MonoGameSpriteFont,
                text: Message,
                position: coordinates - Vector2.UnitY,
                color: outlineColor.Value,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 1f / (coordinates.Y * (int) Layer.OverlayEffect - 1)
            );
        }
    }
}