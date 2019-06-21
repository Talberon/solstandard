using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Window.Content.Combat
{
    public class Die : IRenderable, ICombatPoint
    {
        public enum FaceValue
        {
            Blank,
            Shield,
            Sword
        }

        public enum DieSides
        {
            One,
            Two,
            Three,
            Four,
            Five,
            Six
        }

        private static readonly Dictionary<DieSides, FaceValue> DieValues =
            new Dictionary<DieSides, FaceValue>
            {
                {DieSides.One, FaceValue.Blank},
                {DieSides.Two, FaceValue.Shield},
                {DieSides.Three, FaceValue.Shield},
                {DieSides.Four, FaceValue.Sword},
                {DieSides.Five, FaceValue.Sword},
                {DieSides.Six, FaceValue.Sword}
            };

        private readonly SpriteAtlas dieAtlas;
        private DieSides currentSide;
        public Color DefaultColor { get; set; }
        public bool Enabled { get; private set; }

        public Die(DieSides initialSide, int size, Color color)
        {
            currentSide = initialSide;
            DefaultColor = color;
            dieAtlas = new SpriteAtlas(AssetManager.DiceTexture, new Vector2(AssetManager.DiceTexture.Height),
                new Vector2(size));
            Enabled = true;
        }

        public int Height => dieAtlas.Height;

        public int Width => dieAtlas.Width;

        public void Roll()
        {
            int randomValue = GameDriver.Random.Next(0, 6);
            currentSide = (DieSides) randomValue;
            dieAtlas.SetCellIndex((int) currentSide);
        }

        public FaceValue GetFaceValue()
        {
            return DieValues[currentSide];
        }

        public void Disable(Color disabledColor)
        {
            Enabled = false;
            DefaultColor = disabledColor;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            dieAtlas.Draw(spriteBatch, position, colorOverride);
        }

        public IRenderable Clone()
        {
            return new Die(currentSide, Height, DefaultColor);
        }

        public override string ToString()
        {
            return "Die: {Value=" + currentSide + ", Atlas=" + dieAtlas + "}";
        }
    }
}