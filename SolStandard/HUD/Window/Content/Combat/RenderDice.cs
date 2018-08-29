using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Combat
{
    public class RenderDice : IRenderable
    {
        private readonly List<Die> dice;
        private readonly int rowSize;
        private readonly SpriteAtlas dieSprite;
        public int Height { get; private set; }
        public int Width { get; private set; }

        public RenderDice(int diceToRoll, int rowSize)
        {
            this.rowSize = rowSize;
            dieSprite = new SpriteAtlas(GameDriver.DiceTexture, GameDriver.DiceTexture.Height, 1);
            dice = PopulateDice(dieSprite, diceToRoll);
            Height = CalculateHeight();
            Width = CalculateWidth();
        }

        private static List<Die> PopulateDice(SpriteAtlas dieSprite, int diceToRoll)
        {
            List<Die> diceToGenerate = new List<Die>();

            for (int i = 0; i < diceToRoll; i++)
            {
                diceToGenerate.Add(new Die(Die.DieFaces.One));
            }

            return diceToGenerate;
        }

        private int CalculateHeight()
        {
            int totalRows = (int) Math.Ceiling((float) dice.Count / rowSize);
            return totalRows * GameDriver.DiceTexture.Height;
        }

        private int CalculateWidth()
        {
            //FIXME using the height is cheating and will only work on an atlas with all sprites on one row
            return rowSize * GameDriver.DiceTexture.Height;
        }

        public void RollDice()
        {
            foreach (Die die in dice)
            {
                die.Roll();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            Vector2 dieOffset = new Vector2();

            for (int i = 0; i < dice.Count; i++)
            {
                //Drop dice to the next row after rowSize reached.
                if (i != 0 && i % rowSize == 0)
                {
                    dieOffset.X = 0;
                    dieOffset.Y += dice[i].Height;
                }

                dice[i].Draw(spriteBatch, position + dieOffset, color);

                dieOffset.X += dice[i].Width;
            }
        }

        public override string ToString()
        {
            string output = "RenderDice: {Dice: <";
            output += string.Join(",", dice);
            output += ">}";

            return output;
        }
    }
}