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
        private static readonly Color DefaultDieColor = Color.White;
        public static readonly Color BonusDieColor = new Color(80, 200, 80);
        public static readonly Color IgnoredDieColor = new Color(80, 80, 80, 180);
        public static readonly Color DamageDieColor = new Color(200, 50, 50, 180);
        public static readonly Color BlockedDieColor = new Color(50, 50, 150, 180);
        public Color DefaultColor { get; set; }

        private readonly List<Die> dice;
        private readonly int maxRowSize;
        private readonly int dieSize;
        private readonly int baseDice;
        private readonly int bonusDice;

        public CombatDice(int baseDice, int bonusDice, int maxRowSize, int dieSize)
        {
            this.baseDice = (baseDice > 0) ? baseDice : 0;
            this.bonusDice = (bonusDice > 0) ? bonusDice : 0;
            this.dieSize = dieSize;

            this.maxRowSize = maxRowSize;
            dice = PopulateDice();
            DefaultColor = Color.White;
        }

        private List<Die> PopulateDice()
        {
            var diceToGenerate = new List<Die>();

            for (int i = 0; i < baseDice; i++)
            {
                diceToGenerate.Add(new Die(Die.DieSides.One, dieSize, DefaultDieColor));
            }

            for (int i = 0; i < bonusDice; i++)
            {
                diceToGenerate.Add(new Die(Die.DieSides.One, dieSize, BonusDieColor));
            }

            return diceToGenerate;
        }

        public int Height
        {
            get
            {
                if (dice.Count < 1) return 0;
                
                int totalRows = (int) Math.Ceiling((float) dice.Count / maxRowSize);
                return totalRows * dice.First().Height;
            }
        }

        public int Width
        {
            get
            {
                if (dice.Count < 1) return 0;
                
                int rowSize = (dice.Count > maxRowSize) ? maxRowSize : dice.Count;
                return rowSize * dice.First().Width;
            }
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
            var dieOffset = new Vector2();

            for (int i = 0; i < dice.Count; i++)
            {
                //Drop dice to the next row after maxRowSize reached.
                if (i != 0 && i % maxRowSize == 0)
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
            throw new InvalidOperationException("Do not draw combat dice all of the same color!");
        }

        public IRenderable Clone()
        {
            return new CombatDice(baseDice, bonusDice, maxRowSize, dieSize);
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