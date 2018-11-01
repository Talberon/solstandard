using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit
{
    public enum StatIcons
    {
        None,
        Hp,
        Atk,
        Def,
        Mv,
        Crosshair,
        EmptyHp,
        Coin,
        EmptyDef,
        Positive,
        Negative
    }


    public class UnitStatistics
    {
        private const int IconSizePixels = 16;

        public int MaxHp { get; private set; }
        public int MaxArmor { get; private set; }
        public int BaseAtk { get; private set; }
        public int BaseLuck { get; set; }
        public int BaseMv { get; private set; }
        public int[] BaseAtkRange { get; private set; }

        public int Hp { get; set; }
        public int Atk { get; set; }
        public int Armor { get; set; }
        public int Luck { get; set; }
        public int Mv { get; set; }
        public int[] AtkRange { get; set; }

        public UnitStatistics(int hp, int armor, int atk, int luck, int mv, int[] atkRange)
        {
            Hp = hp;
            Armor = armor;
            Atk = atk;
            Luck = luck;
            Mv = mv;
            AtkRange = atkRange;

            MaxHp = hp;
            MaxArmor = armor;
            BaseAtk = atk;
            BaseLuck = luck;
            BaseMv = mv;
            BaseAtkRange = ArrayDeepCopier<int>.DeepCopyArray(atkRange);
        }


        public static SpriteAtlas GetSpriteAtlas(StatIcons stat)
        {
            return GetSpriteAtlas(stat, new Vector2(GameDriver.CellSize));
        }

        public static SpriteAtlas GetSpriteAtlas(StatIcons stat, Vector2 size)
        {
            return new SpriteAtlas(AssetManager.StatIcons, new Vector2(IconSizePixels), size, (int) stat);
        }

        public override string ToString()
        {
            string output = "";

            output += "HP: " + Hp.ToString() + "/" + MaxHp;
            output += Environment.NewLine;
            output += "ARM: " + Armor.ToString() + "/" + MaxArmor;
            output += Environment.NewLine;
            output += "ATK: " + Atk.ToString() + "/" + BaseAtk;
            output += Environment.NewLine;
            output += "LCK: " + Luck.ToString() + "/" + BaseLuck;
            output += Environment.NewLine;
            output += "MV: " + Mv.ToString() + "/" + BaseMv;
            output += Environment.NewLine;
            output += string.Format("RNG: [{0}]/[{1}]", string.Join(",", AtkRange), string.Join(",", BaseAtkRange));

            return output;
        }

        public static Color DetermineStatColor(int stat, int baseStat)
        {
            if (stat > baseStat) return GameContext.PositiveColor;
            if (stat == baseStat) return GameContext.NeutralColor;
            if (stat < baseStat) return GameContext.NegativeColor;

            return Color.White;
        }
    }
}