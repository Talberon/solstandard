using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using Color = Microsoft.Xna.Framework.Color;

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
        private Color color;

        public bool Enabled { get; private set; }

        public Die(DieSides initialSide, int size, Color color)
        {
            currentSide = initialSide;
            this.color = color;
            dieAtlas = new SpriteAtlas(AssetManager.DiceTexture, new Vector2(AssetManager.DiceTexture.Height),
                new Vector2(size));
            Enabled = true;
        }

        public int Height
        {
            get { return dieAtlas.Height; }
        }

        public int Width
        {
            get { return dieAtlas.Width; }
        }

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
            color = disabledColor;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, color);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            dieAtlas.Draw(spriteBatch, position, colorOverride);
        }

        public IRenderable Clone()
        {
            return new Die(currentSide, Height, color);
        }

        public override string ToString()
        {
            return "Die: {Value=" + currentSide + ", Atlas=" + dieAtlas + "}";
        }
    }
}