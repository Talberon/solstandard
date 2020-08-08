using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class MiniHealthBar : IHealthBar
    {
        private static readonly Color ShieldColor = new Color(180, 180, 200);
        private static readonly Color HealthColor = new Color(50, 255, 50);

        private readonly List<IResourcePoint> armorPips;
        private int currentArmor;

        private readonly List<IResourcePoint> healthPips;
        private int currentHp;

        private Vector2 barSize;

        private readonly int maxArmor;
        private readonly int maxHp;

        public MiniHealthBar(int maxArmor, int maxHp, Vector2 barSize)
        {
            this.maxHp = maxHp;
            this.maxArmor = maxArmor;

            currentArmor = maxArmor;
            armorPips = GenerateArmorPips(maxArmor);
            UpdatePips(armorPips, currentArmor);

            currentHp = maxHp;
            healthPips = GenerateHpPips(maxHp);
            UpdatePips(healthPips, currentHp);

            this.barSize = barSize;
        }

        private Vector2 GetPipSize(int maxStatValue)
        {
            return new Vector2((float) Math.Floor(barSize.X / maxStatValue), barSize.Y / 2);
        }

        private List<IResourcePoint> GenerateArmorPips(int maxValue)
        {
            var pips = new List<IResourcePoint>();

            for (int i = 0; i < maxValue; i++)
            {
                pips.Add(new BarPoint(GetPipSize(maxValue), ShieldColor, Color.Transparent));
            }

            return pips;
        }

        private List<IResourcePoint> GenerateHpPips(int maxValue)
        {
            var pips = new List<IResourcePoint>();

            for (int i = 0; i < maxValue; i++)
            {
                pips.Add(new BarPoint(GetPipSize(maxValue), HealthColor, Color.Transparent));
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

        public Vector2 BarSize
        {
            set
            {
                barSize = value;
                armorPips.ForEach(pip => pip.Size = new Vector2(barSize.X / maxArmor, barSize.Y / 2));
                healthPips.ForEach(pip => pip.Size = new Vector2(barSize.X / maxHp, barSize.Y / 2));
            }
        }

        public void SetArmorAndHp(int armor, int hp)
        {
            currentArmor = armor;
            UpdatePips(armorPips, currentArmor);

            currentHp = hp;
            UpdatePips(healthPips, currentHp);
        }

        public int Height => Convert.ToInt32(barSize.Y);

        public int Width => Convert.ToInt32(barSize.X);

        public Color DefaultColor
        {
            get
            {
                int red = 255 - Convert.ToInt32(255 * ((float) currentHp / maxHp));
                int green = Convert.ToInt32(255 * ((float) currentHp / maxHp));
                const int blue = 0;
                return new Color(red, green, blue);
            }
            set => throw new InvalidOperationException("Cannot set health bar color.");
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            var pipOffset = new Vector2(position.X, position.Y);

            foreach (IResourcePoint pip in armorPips)
            {
                pip.Draw(spriteBatch, pipOffset);
                pipOffset.X += pip.Width;
            }

            pipOffset.X = position.X;
            pipOffset.Y += GetPipSize(maxArmor).Y;

            foreach (IResourcePoint pip in healthPips)
            {
                pip.Draw(spriteBatch, pipOffset, colorOverride);
                pipOffset.X += pip.Width;
            }
        }

        public IRenderable Clone()
        {
            return new MiniHealthBar(maxArmor, maxHp, barSize);
        }
    }
}