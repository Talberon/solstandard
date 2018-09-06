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
        public static readonly Color DefaultDieColor = Color.White;
        public static readonly Color BonusDieColor = new Color(80, 200, 80);
        public static readonly Color IgnoredDieColor = new Color(80, 80, 80, 180);
        public static readonly Color DamageDieColor = new Color(200, 50, 50, 180);
        public static readonly Color BlockedDieColor = new Color(50, 50, 150, 180);

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
                diceToGenerate.Add(new Die(Die.DieSides.One, DefaultDieColor));
            }

            for (int i = 0; i < bonusDice; i++)
            {
                diceToGenerate.Add(new Die(Die.DieSides.One, BonusDieColor));
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

        public int CountFaceValue(Die.FaceValue faceValue, bool enabledOnly)
        {
            int faceCount = 0;
            foreach (Die die in dice)
            {
                if (die.GetFaceValue() == faceValue)
                {
                    if (enabledOnly)
                    {
                        if (die.Enabled)
                        {
                            faceCount++;
                        }
                    }
                    else
                    {
                        faceCount++;
                    }
                }
            }

            return faceCount;
        }

        public void DisableAllDiceWithValue(Die.FaceValue faceValue)
        {
            int totalFaceValues = CountFaceValue(faceValue, true);
            for (int i = 0; i < totalFaceValues; i++)
            {
                DisableNextDieWithValue(faceValue, IgnoredDieColor);
            }
        }

        public void DisableNextDieWithValue(Die.FaceValue faceValue, Color dieColor)
        {
            foreach (Die die in dice)
            {
                if (die.GetFaceValue() != faceValue) continue;
                if (!die.Enabled) continue;
                die.Disable(dieColor);
                return;
            }
        }

        public void ResolveDamageNextDieWithValue(Die.FaceValue faceValue)
        {
            DisableNextDieWithValue(faceValue, DamageDieColor);
        }

        public void BlockNextDieWithValue(Die.FaceValue faceValue)
        {
            DisableNextDieWithValue(faceValue, BlockedDieColor);
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

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
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

                dice[i].Draw(spriteBatch, position + dieOffset, colorOverride);

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