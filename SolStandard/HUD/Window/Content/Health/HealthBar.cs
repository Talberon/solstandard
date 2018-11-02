using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class HealthBar : IRenderable
    {
        private readonly List<IResourcePoint> armorPips;
        private int currentArmor;

        private readonly List<IResourcePoint> healthPips;
        private int currentHp;

        private const int MaxPointsPerRow = 10;
        private Vector2 pipSize;
        private Vector2 barSize;

        public HealthBar(int maxArmor, int maxHp, Vector2 barSize)
        {
            currentArmor = maxArmor;
            armorPips = GenerateArmorPips(maxArmor);
            UpdatePips(armorPips, currentArmor);

            currentHp = maxHp;
            healthPips = GenerateHpPips(maxHp);
            UpdatePips(healthPips, currentHp);

            BarSize = barSize;
        }

        private Vector2 PipSize
        {
            set
            {
                pipSize = value;

                foreach (IResourcePoint pip in healthPips)
                {
                    pip.Size = value;
                }

                foreach (IResourcePoint pip in armorPips)
                {
                    pip.Size = value;
                }
            }
        }

        public Vector2 BarSize
        {
            set
            {
                barSize = value;

                float colCount = ColumnCount;
                float widthLimit = barSize.X / colCount;

                float heightLimit = barSize.Y / (HealthRowCount + ArmorRowCount);

                PipSize = new Vector2((widthLimit > heightLimit) ? heightLimit : widthLimit);
            }
        }

        private float ColumnCount
        {
            get
            {
                List<IResourcePoint> greaterPips = (healthPips.Count > armorPips.Count) ? healthPips : armorPips;
                float colCount = (greaterPips.Count > MaxPointsPerRow) ? MaxPointsPerRow : greaterPips.Count;
                return colCount;
            }
        }

        private float ArmorRowCount
        {
            get
            {
                float armorRowCount = Convert.ToSingle(Math.Ceiling((float) armorPips.Count / MaxPointsPerRow));
                return armorRowCount;
            }
        }

        private float HealthRowCount
        {
            get
            {
                float healthRowCount = Convert.ToSingle(Math.Ceiling((float) healthPips.Count / MaxPointsPerRow));
                return healthRowCount;
            }
        }

        private List<IResourcePoint> GenerateArmorPips(int maxArmor)
        {
            List<IResourcePoint> pips = new List<IResourcePoint>();

            for (int i = 0; i < maxArmor; i++)
            {
                pips.Add(new ArmorPoint(pipSize));
            }

            return pips;
        }

        private List<IResourcePoint> GenerateHpPips(int maxHp)
        {
            List<IResourcePoint> pips = new List<IResourcePoint>();

            for (int i = 0; i < maxHp; i++)
            {
                pips.Add(new Heart(pipSize));
            }

            return pips;
        }

        private static void UpdatePips(IReadOnlyList<IResourcePoint> pips, int currentResource)
        {
            for (int i = 0; i < pips.Count; i++)
            {
                pips[i].Active = i <= (currentResource - 1);
            }
        }

        public int Height
        {
            get { return Convert.ToInt32(barSize.Y); }
        }

        public int Width
        {
            get { return Convert.ToInt32(barSize.X); }
        }

        public void DealDamage()
        {
            if (currentArmor > 0)
            {
                currentArmor--;
                UpdatePips(armorPips, currentArmor);
            }
            else
            {
                currentHp--;
                UpdatePips(healthPips, currentHp);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            Vector2 pipOffset = Vector2.Zero;

            DrawPips(spriteBatch, position, armorPips, pipOffset);

            pipOffset.Y += pipSize.Y * ArmorRowCount;

            DrawPips(spriteBatch, position, healthPips, pipOffset);
        }

        private void DrawPips(SpriteBatch spriteBatch, Vector2 position, List<IResourcePoint> points, Vector2 pipOffset)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Draw(spriteBatch, position + pipOffset);

                pipOffset.X += (barSize.X / ColumnCount);

                if ((i + 1) % MaxPointsPerRow != 0) continue;

                pipOffset.Y += pipSize.Y;
                pipOffset.X = 0;
            }
        }
    }
}