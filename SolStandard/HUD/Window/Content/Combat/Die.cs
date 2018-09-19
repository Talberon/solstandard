using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Window.Content.Combat
{
    public class Die : IRenderable
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

        public int Height { get; private set; }
        public int Width { get; private set; }
        public bool Enabled { get; private set; }

        public Die(DieSides initialSide, Color color)
        {
            currentSide = initialSide;
            this.color = color;
            dieAtlas = new SpriteAtlas(AssetManager.DiceTexture, new Vector2(AssetManager.DiceTexture.Height), 1);
            Height = dieAtlas.Height;
            Width = dieAtlas.Width;
            Enabled = true;
        }

        public Die(DieSides initialSide) : this(initialSide, Color.White)
        {
            //Intentionally left blank
        }

        public void Roll()
        {
            int randomValue = GameDriver.Random.Next(0, 6);
            currentSide = (DieSides) randomValue;
            dieAtlas.CellIndex = (int) currentSide + 1;
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

        public override string ToString()
        {
            return "Die: {Value=" + currentSide + ", Atlas=" + dieAtlas + "}";
        }
    }
}