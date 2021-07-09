using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General.Item;
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
        Retribution,
        Block,
        CommandPoints,
        EmptyCommandPoints
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
            {Stats.Retribution, "RET"},
            {Stats.Block, "BLK"},
            {Stats.CommandPoints, "CP"}
        };

        private const int IconSizePixels = 16;

        public int MaxHP { get; }
        public int MaxArmor { get; }
        public int MaxCmd { get; }
        public int[] BaseAtkRange { get; }

        public int BaseAtk { get; }
        public int BaseRet { get; }
        public int BaseLuck { get; }
        public int BaseMv { get; }
        public int BaseBlk { get; }

        public int CurrentHP { get; set; }
        public int CurrentArmor { get; set; }
        public int CurrentCmd { get; set; }
        public int[] CurrentAtkRange { get; set; }

        public int AtkModifier { get; set; }
        public int RetModifier { get; set; }
        public int LuckModifier { get; set; }
        public int MvModifier { get; set; }
        public int BlkModifier { get; set; }


        public UnitStatistics(int hp, int armor, int atk, int ret, int blk, int luck, int mv, int[] atkRange,
            int maxCmd) :
            this(
                maxHP: hp,
                maxArmor: armor,
                baseAtk: atk,
                baseRet: ret,
                baseBlk: blk,
                baseLuck: luck,
                baseMv: mv,
                baseAtkRange: atkRange,
                currentHP: hp,
                currentArmor: armor,
                atkModifier: 0,
                retModifier: 0,
                blkModifier: 0,
                luckModifier: 0,
                mvModifier: 0,
                currentAtkRange: atkRange,
                maxCmd: maxCmd
            )
        {
        }

        private UnitStatistics(int maxHP, int maxArmor, int baseAtk, int baseRet, int baseBlk, int baseLuck, int baseMv,
            int[] baseAtkRange, int currentHP, int currentArmor, int atkModifier, int retModifier, int blkModifier,
            int luckModifier, int mvModifier, int[] currentAtkRange, int maxCmd
        )
        {
            CurrentHP = currentHP;
            CurrentArmor = currentArmor;
            CurrentAtkRange = ArrayDeepCopier<int>.DeepCopyArray(currentAtkRange);

            AtkModifier = atkModifier;
            RetModifier = retModifier;
            LuckModifier = luckModifier;
            BlkModifier = blkModifier;
            MvModifier = mvModifier;

            MaxHP = maxHP;
            MaxArmor = maxArmor;
            MaxCmd = maxCmd;
            BaseAtkRange = ArrayDeepCopier<int>.DeepCopyArray(baseAtkRange);

            BaseAtk = baseAtk;
            BaseRet = baseRet;
            BaseBlk = baseBlk;
            BaseLuck = baseLuck;
            BaseMv = baseMv;
        }

        public UnitStatistics ApplyWeaponStatistics(WeaponStatistics weaponStatistics, bool ignoreModifiers = false)
        {
            return new UnitStatistics(
                maxHP: MaxHP,
                maxArmor: MaxArmor,
                baseAtk: weaponStatistics.AtkValue,
                baseRet: BaseRet,
                baseBlk: BaseBlk,
                baseLuck: BaseLuck,
                baseMv: BaseMv,
                baseAtkRange: BaseAtkRange,
                currentHP: CurrentHP,
                currentArmor: CurrentArmor,
                atkModifier: (ignoreModifiers) ? 0 : AtkModifier,
                retModifier: RetModifier,
                blkModifier: BlkModifier,
                luckModifier: weaponStatistics.LuckModifier,
                mvModifier: (ignoreModifiers) ? 0 : MvModifier,
                currentAtkRange: weaponStatistics.AtkRange,
                maxCmd: MaxCmd
            );
        }

        public int Atk => BaseAtk + AtkModifier;
        public int Ret => BaseRet + RetModifier;
        public int Luck => BaseLuck + LuckModifier;
        public int Mv => BaseMv + MvModifier;
        public int Blk => BaseBlk + BlkModifier;

        public static SpriteAtlas GetSpriteAtlas(Stats stat)
        {
            return GetSpriteAtlas(stat, GameDriver.CellSizeVector);
        }

        public static SpriteAtlas GetSpriteAtlas(Stats stat, Vector2 size)
        {
            return new SpriteAtlas(AssetManager.StatIcons, new Vector2(IconSizePixels), size, (int) stat);
        }

        public override string ToString()
        {
            string output = string.Empty;

            output += Abbreviation[Stats.Hp] + ": " + CurrentHP + "/" + MaxHP;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Armor] + ": " + CurrentArmor + "/" + MaxArmor;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Atk] + ": " + Atk + "/" + BaseAtk;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Retribution] + ": " + Ret + "/" + BaseRet;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Luck] + ": " + Luck + "/" + BaseLuck;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Block] + ": " + Blk + "/" + BaseBlk;
            output += Environment.NewLine;
            output += Abbreviation[Stats.Mv] + ": " + Mv + "/" + BaseMv;
            output += Environment.NewLine;
            output += Abbreviation[Stats.CommandPoints] + ": " + CurrentCmd + "/" + MaxCmd;
            output += Environment.NewLine;
            output += string.Format(Abbreviation[Stats.AtkRange] + ": [{0}]/[{1}]", string.Join(",", CurrentAtkRange),
                string.Join(",", BaseAtkRange));

            return output;
        }

        public static Color DetermineStatColor(int stat, int baseStat)
        {
            if (stat > baseStat) return GlobalContext.PositiveColor;
            if (stat == baseStat) return GlobalContext.NeutralColor;
            if (stat < baseStat) return GlobalContext.NegativeColor;

            return Color.White;
        }

        public override bool Equals(object obj)
        {
            return obj is UnitStatistics other && Equals(other);
        }

        private bool Equals(UnitStatistics other)
        {
            return MaxHP == other.MaxHP &&
                   MaxArmor == other.MaxArmor &&
                   Equals(BaseAtkRange, other.BaseAtkRange) &&
                   BaseAtk == other.BaseAtk &&
                   BaseRet == other.BaseRet &&
                   BaseBlk == other.BaseBlk &&
                   BaseLuck == other.BaseLuck &&
                   BaseMv == other.BaseMv &&
                   CurrentHP == other.CurrentHP &&
                   CurrentArmor == other.CurrentArmor &&
                   Equals(CurrentAtkRange, other.CurrentAtkRange) &&
                   AtkModifier == other.AtkModifier &&
                   RetModifier == other.RetModifier &&
                   LuckModifier == other.LuckModifier &&
                   MvModifier == other.MvModifier &&
                   MaxCmd == other.MaxCmd &&
                   CurrentCmd == other.CurrentCmd;
        }

        // ReSharper disable NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = MaxHP;
                hashCode += (hashCode * 397) ^ MaxArmor;
                hashCode += (hashCode * 397) ^ (BaseAtkRange != null ? BaseAtkRange.GetHashCode() : 0);
                hashCode += (hashCode * 397) ^ BaseAtk;
                hashCode += (hashCode * 397) ^ BaseRet;
                hashCode += (hashCode * 397) ^ BaseLuck;
                hashCode += (hashCode * 397) ^ BaseBlk;
                hashCode += (hashCode * 397) ^ BaseMv;
                hashCode += (hashCode * 397) ^ MaxCmd;

                hashCode += (hashCode * 397) ^ CurrentHP;
                hashCode += (hashCode * 397) ^ CurrentArmor;
                hashCode += (hashCode * 397) ^ (CurrentAtkRange != null ? CurrentAtkRange.GetHashCode() : 0);
                hashCode += (hashCode * 397) ^ AtkModifier;
                hashCode += (hashCode * 397) ^ RetModifier;
                hashCode += (hashCode * 397) ^ LuckModifier;
                hashCode += (hashCode * 397) ^ MvModifier;
                hashCode += (hashCode * 397) ^ CurrentCmd;
                return hashCode;
            }
        }
    }
}