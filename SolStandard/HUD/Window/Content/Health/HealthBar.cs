using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class HealthBar : IHealthBar
    {
        private readonly List<IResourcePoint> armorPips;
        private readonly int maxArmor;
        private int currentArmor;

        private readonly List<IResourcePoint> healthPips;
        private readonly int maxHp;
        private int currentHp;

        private const int MaxPointsPerRow = 10;
        private Vector2 pipSize;
        private Vector2 barSize;
        public Color DefaultColor { get; set; }

        public HealthBar(int maxArmor, int maxHp, Vector2 barSize)
        {
            this.maxArmor = maxArmor;
            currentArmor = maxArmor;
            armorPips = GenerateArmorPips(maxArmor);
            UpdatePips(armorPips, currentArmor);

            this.maxHp = maxHp;
            currentHp = maxHp;
            healthPips = GenerateHpPips(maxHp);
            UpdatePips(healthPips, currentHp);

            BarSize = barSize;
            DefaultColor = Color.White;
        }

        private List<IResourcePoint> GenerateArmorPips(int maxPips)
        {
            List<IResourcePoint> pips = new List<IResourcePoint>();

            for (int i = 0; i < maxPips; i++)
            {
                pips.Add(new ArmorPoint(pipSize));
            }

            return pips;
        }

        private List<IResourcePoint> GenerateHpPips(int maxPips)
        {
            List<IResourcePoint> pips = new List<IResourcePoint>();

            for (int i = 0; i < maxPips; i++)
            {
                pips.Add(new HeartPoint(pipSize));
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

        public int Height
        {
            get { return Convert.ToInt32(barSize.Y); }
        }

        public int Width
        {
            get { return Convert.ToInt32(barSize.X); }
        }

        public void Update(int armor, int hp)
        {
            currentArmor = armor;
            UpdatePips(armorPips, currentArmor);

            currentHp = hp;
            UpdatePips(healthPips, currentHp);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            Vector2 pipOffset = Vector2.Zero;

            DrawPips(spriteBatch, position, armorPips, pipOffset);

            pipOffset.Y += pipSize.Y * ArmorRowCount;

            DrawPips(spriteBatch, position, healthPips, pipOffset);
        }


        public IRenderable Clone()
        {
            return new HealthBar(maxArmor, maxHp, barSize);
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