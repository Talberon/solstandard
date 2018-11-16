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
        Negative,
        Retribution
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
            {Stats.Luck, "LCK"},
            {Stats.Retribution, "RET"}
        };


        private const int IconSizePixels = 16;

        public int MaxHP { get; private set; }
        public int MaxArmor { get; private set; }
        public int[] BaseAtkRange { get; private set; }

        public int BaseAtk { get; private set; }
        public int BaseRet { get; private set; }
        public int BaseLuck { get; private set; }
        public int BaseMv { get; private set; }

        public int CurrentHP { get; set; }
        public int CurrentArmor { get; set; }
        public int[] CurrentAtkRange { get; set; }

        public int AtkModifier { get; set; }
        public int RetModifier { get; set; }
        public int LuckModifier { get; set; }
        public int MvModifier { get; set; }


        public UnitStatistics(int currentHp, int currentArmor, int atk, int ret, int luck, int mv,
            int[] currentAtkRange)
        {
            CurrentHP = currentHp;
            CurrentArmor = currentArmor;
            CurrentAtkRange = currentAtkRange;

            AtkModifier = 0;
            RetModifier = 0;
            LuckModifier = 0;
            MvModifier = 0;

            MaxHP = currentHp;
            MaxArmor = currentArmor;
            BaseAtkRange = ArrayDeepCopier<int>.DeepCopyArray(currentAtkRange);

            BaseAtk = atk;
            BaseRet = ret;
            BaseLuck = luck;
            BaseMv = mv;
        }

        public int Atk
        {
            get { return BaseAtk + AtkModifier; }
        }

        public int Ret
        {
            get { return BaseRet + RetModifier; }
        }

        public int Luck
        {
            get { return BaseLuck + LuckModifier; }
        }

        public int Mv
        {
            get { return BaseMv + MvModifier; }
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
            string output = string.Empty;

            output += Abbreviation[Stats.Hp] + ": " + CurrentHP.ToString() + "/" + MaxHP;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Armor] + ": " + CurrentArmor.ToString() + "/" + MaxArmor;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Atk] + ": " + Atk.ToString() + "/" + BaseAtk;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Retribution] + ": " + Ret.ToString() + "/" + BaseRet;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Luck] + ": " + Luck.ToString() + "/" + BaseLuck;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Mv] + ": " + Mv.ToString() + "/" + BaseMv;
            output += Environment.NewLine;
            output += string.Format(Abbreviation[Stats.AtkRange] + ": [{0}]/[{1}]", string.Join(",", CurrentAtkRange),
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