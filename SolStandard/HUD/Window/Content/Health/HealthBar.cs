using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class HealthBar : IHealthBar
    {
        protected readonly List<IResourcePoint> ArmorPips;
        private readonly int maxArmor;
        private int currentArmor;

        protected readonly List<IResourcePoint> HealthPips;
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
            ArmorPips = GenerateArmorPips(maxArmor);
            UpdatePips(ArmorPips, currentArmor);

            this.maxHp = maxHp;
            currentHp = maxHp;
            HealthPips = GenerateHpPips(maxHp);
            UpdatePips(HealthPips, currentHp);

            BarSize = barSize;
            DefaultColor = Color.White;
        }

        protected virtual void AddArmorPoint(List<IResourcePoint> points)
        {
            points.Add(
                new ResourcePoint(
                    pipSize,
                    UnitStatistics.GetSpriteAtlas(Stats.Armor),
                    UnitStatistics.GetSpriteAtlas(Stats.EmptyArmor)
                )
            );
        }

        protected virtual void AddHealthPoint(List<IResourcePoint> points)
        {
            points.Add(
                new ResourcePoint(
                    pipSize,
                    UnitStatistics.GetSpriteAtlas(Stats.Hp),
                    UnitStatistics.GetSpriteAtlas(Stats.EmptyHp)
                )
            );
        }

        private List<IResourcePoint> GenerateArmorPips(int maxPips)
        {
            var pips = new List<IResourcePoint>();

            for (int i = 0; i < maxPips; i++)
            {
                AddArmorPoint(pips);
            }

            return pips;
        }

        private List<IResourcePoint> GenerateHpPips(int maxPips)
        {
            var pips = new List<IResourcePoint>();

            for (int i = 0; i < maxPips; i++)
            {
                AddHealthPoint(pips);
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

                foreach (IResourcePoint pip in HealthPips)
                {
                    pip.Size = value;
                }

                foreach (IResourcePoint pip in ArmorPips)
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
                List<IResourcePoint> greaterPips = (HealthPips.Count > ArmorPips.Count) ? HealthPips : ArmorPips;
                float colCount = (greaterPips.Count > MaxPointsPerRow) ? MaxPointsPerRow : greaterPips.Count;
                return colCount;
            }
        }

        private float ArmorRowCount
        {
            get
            {
                float armorRowCount = Convert.ToSingle(Math.Ceiling((float) ArmorPips.Count / MaxPointsPerRow));
                return armorRowCount;
            }
        }

        private float HealthRowCount
        {
            get
            {
                float healthRowCount = Convert.ToSingle(Math.Ceiling((float) HealthPips.Count / MaxPointsPerRow));
                return healthRowCount;
            }
        }

        public int Height => Convert.ToInt32(barSize.Y);

        public int Width => Convert.ToInt32(barSize.X);

        public void SetArmorAndHp(int armor, int hp)
        {
            currentArmor = armor;
            UpdatePips(ArmorPips, currentArmor);

            currentHp = hp;
            UpdatePips(HealthPips, currentHp);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            Vector2 pipOffset = Vector2.Zero;

            DrawPips(spriteBatch, position, ArmorPips, pipOffset);

            pipOffset.Y += pipSize.Y * ArmorRowCount;

            DrawPips(spriteBatch, position, HealthPips, pipOffset);
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