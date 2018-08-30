using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Combat
{
    public class CombatDice : IRenderable
    {
        private static readonly Color BonusDieColor = new Color(20,180,20);
        
        private readonly List<Die> dice;
        private readonly int rowSize;
        public int Height { get; private set; }
        public int Width { get; private set; }

        public CombatDice(int baseDice, int bonusDice, int rowSize)
        {
            if (baseDice < 1) throw new ArgumentOutOfRangeException();
            if (bonusDice < 0) throw new ArgumentOutOfRangeException();

            this.rowSize = rowSize;
            dice = PopulateDice(baseDice, bonusDice);
            Height = CalculateHeight();
            Width = CalculateWidth();
        }

        private static List<Die> PopulateDice(int baseDice, int bonusDice)
        {
            List<Die> diceToGenerate = new List<Die>();

            for (int i = 0; i < baseDice; i++)
            {
                diceToGenerate.Add(new Die(Die.DieFaces.One));
            }

            for (int i = 0; i < bonusDice; i++)
            {
                diceToGenerate.Add(new Die(Die.DieFaces.One, BonusDieColor));
            }

            return diceToGenerate;
        }

        private int CalculateHeight()
        {
            int totalRows = (int) Math.Ceiling((float) dice.Count / rowSize);
            return totalRows * dice.First().Height;
        }

        private int CalculateWidth()
        {
            return rowSize * dice.First().Width;
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
            Vector2 dieOffset = new Vector2();

            for (int i = 0; i < dice.Count; i++)
            {
                //Drop dice to the next row after rowSize reached.
                if (i != 0 && i % rowSize == 0)
                {
                    dieOffset.X = 0;
                    dieOffset.Y += dice[i].Height;
                }

                dice[i].Draw(spriteBatch, position + dieOffset);

                dieOffset.X += dice[i].Width;
            }
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