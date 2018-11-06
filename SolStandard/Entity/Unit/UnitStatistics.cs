using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit
{
    public enum Stats
    {
        Hp,
        Atk,
        Armor,
        Mv,
        AtkRange,
        EmptyHp,
        Luck,
        EmptyArmor,
        Positive,
        Negative
    }

    public class UnitStatistics
    {
        public static readonly Dictionary<Stats, string> Abbreviation = new Dictionary<Stats, string>
        {
            {Stats.Hp, "HP"},
            {Stats.Atk, "ATK"},
            {Stats.Armor, "AMR"},
            {Stats.Mv, "MV"},
            {Stats.AtkRange, "RNG"},
            {Stats.Luck, "LCK"}
        };


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


        public static SpriteAtlas GetSpriteAtlas(Stats stat)
        {
            return GetSpriteAtlas(stat, new Vector2(GameDriver.CellSize));
        }

        public static SpriteAtlas GetSpriteAtlas(Stats stat, Vector2 size)
        {
            return new SpriteAtlas(AssetManager.StatIcons, new Vector2(IconSizePixels), size, (int) stat);
        }

        public override string ToString()
        {
            string output = "";

            output += Abbreviation[Stats.Hp] + ": " + Hp.ToString() + "/" + MaxHp;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Armor] + ": " + Armor.ToString() + "/" + MaxArmor;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Atk] + ": " + Atk.ToString() + "/" + BaseAtk;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Luck] + ": " + Luck.ToString() + "/" + BaseLuck;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Mv] + ": " + Mv.ToString() + "/" + BaseMv;
            output += Environment.NewLine;
            output += string.Format(Abbreviation[Stats.AtkRange] + ": [{0}]/[{1}]", string.Join(",", AtkRange),
                string.Join(",", BaseAtkRange));

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