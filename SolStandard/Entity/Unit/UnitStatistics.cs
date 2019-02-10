using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

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


        public UnitStatistics(int hp, int armor, int atk, int ret, int luck, int mv, int[] atkRange) : this(
            maxHP: hp,
            maxArmor: armor,
            baseAtk: atk,
            baseRet: ret,
            baseLuck: luck,
            baseMv: mv,
            baseAtkRange: atkRange,
            currentHP: hp,
            currentArmor: armor,
            atkModifier: 0,
            retModifier: 0,
            luckModifier: 0,
            mvModifier: 0,
            currentAtkRange: atkRange
        )
        {
        }

        private UnitStatistics(int maxHP, int maxArmor, int baseAtk, int baseRet, int baseLuck, int baseMv,
            int[] baseAtkRange, int currentHP, int currentArmor, int atkModifier, int retModifier, int luckModifier,
            int mvModifier, int[] currentAtkRange
        )
        {
            CurrentHP = currentHP;
            CurrentArmor = currentArmor;
            CurrentAtkRange = ArrayDeepCopier<int>.DeepCopyArray(currentAtkRange);

            AtkModifier = atkModifier;
            RetModifier = retModifier;
            LuckModifier = luckModifier;
            MvModifier = mvModifier;

            MaxHP = maxHP;
            MaxArmor = maxArmor;
            BaseAtkRange = ArrayDeepCopier<int>.DeepCopyArray(baseAtkRange);

            BaseAtk = baseAtk;
            BaseRet = baseRet;
            BaseLuck = baseLuck;
            BaseMv = baseMv;
        }

        public UnitStatistics ApplyWeaponStatistics(WeaponStatistics weaponStatistics)
        {
            return new UnitStatistics(
                maxHP: MaxHP,
                maxArmor: MaxArmor,
                baseAtk: weaponStatistics.AtkValue,
                baseRet: BaseRet,
                baseLuck: BaseLuck,
                baseMv: BaseMv,
                baseAtkRange: BaseAtkRange,
                currentHP: CurrentHP,
                currentArmor: CurrentArmor,
                atkModifier: AtkModifier,
                retModifier: RetModifier,
                luckModifier: weaponStatistics.LuckModifier,
                mvModifier: MvModifier,
                currentAtkRange: weaponStatistics.AtkRange
            );
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
            ITexture2D statsTexture;

            if (AssetManager.StatIcons == null)
            {
                //TODO Find a cleaner way to test so that this isn't necessary
                Trace.TraceWarning("No texture for StatIcons could be found!");
                int statCount = Enum.GetNames(typeof(Stats)).GetLength(0);
                statsTexture = new BlankTexture((int) size.X * statCount, (int) size.Y * statCount);
            }
            else
            {
                statsTexture = AssetManager.StatIcons;
            }

            return new SpriteAtlas(statsTexture, new Vector2(IconSizePixels), size, (int) stat);
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