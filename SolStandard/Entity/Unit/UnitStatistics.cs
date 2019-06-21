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
        Retribution,
        Block
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
            {Stats.Block, "BLK"}
        };


        private const int CommanderAmrBonus = 0;
        private const int CommanderHpBonus = 5;
        private const int CommanderAtkBonus = 0;
        private const int CommanderRetBonus = 0;
        private const int CommanderLuckBonus = 0;
        private const int CommanderMvBonus = 0;
        private const int CommanderBlkBonus = 0;
        private const int IconSizePixels = 16;

        public int MaxHP { get; }
        public int MaxArmor { get; }
        public int[] BaseAtkRange { get; }

        public int BaseAtk { get; }
        public int BaseRet { get; }
        public int BaseLuck { get; }
        public int BaseMv { get; }
        public int BaseBlk { get; }

        public int CurrentHP { get; set; }
        public int CurrentArmor { get; set; }
        public int[] CurrentAtkRange { get; set; }

        public int AtkModifier { get; set; }
        public int RetModifier { get; set; }
        public int LuckModifier { get; set; }
        public int MvModifier { get; set; }
        public int BlkModifier { get; set; }


        public UnitStatistics(int hp, int armor, int atk, int ret, int blk, int luck, int mv, int[] atkRange) : this(
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
            luckModifier: 0,
            mvModifier: 0,
            currentAtkRange: atkRange
        )
        {
        }

        private UnitStatistics(int maxHP, int maxArmor, int baseAtk, int baseRet, int baseBlk, int baseLuck, int baseMv,
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
            BaseBlk = baseBlk;
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
                baseBlk: BaseBlk,
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

        public int Atk => BaseAtk + AtkModifier;
        public int Ret => BaseRet + RetModifier;
        public int Luck => BaseLuck + LuckModifier;
        public int Mv => BaseMv + MvModifier;
        public int Blk => BaseBlk + BlkModifier;

        public UnitStatistics ApplyCommanderBonuses()
        {
            return new UnitStatistics(
                hp: MaxHP + CommanderHpBonus,
                armor: MaxArmor + CommanderAmrBonus,
                atk: Atk + CommanderAtkBonus,
                ret: Ret + CommanderRetBonus,
                blk: Blk + CommanderBlkBonus,
                luck: Luck + CommanderLuckBonus,
                mv: Mv + CommanderMvBonus,
                atkRange: BaseAtkRange
            );
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
                   MvModifier == other.MvModifier;
        }

        // ReSharper disable NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = MaxHP;
                hashCode = (hashCode * 397) ^ MaxArmor;
                hashCode = (hashCode * 397) ^ (BaseAtkRange != null ? BaseAtkRange.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ BaseAtk;
                hashCode = (hashCode * 397) ^ BaseRet;
                hashCode = (hashCode * 397) ^ BaseLuck;
                hashCode = (hashCode * 397) ^ BaseBlk;
                hashCode = (hashCode * 397) ^ BaseMv;

                hashCode = (hashCode * 397) ^ CurrentHP;
                hashCode = (hashCode * 397) ^ CurrentArmor;
                hashCode = (hashCode * 397) ^ (CurrentAtkRange != null ? CurrentAtkRange.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AtkModifier;
                hashCode = (hashCode * 397) ^ RetModifier;
                hashCode = (hashCode * 397) ^ LuckModifier;
                hashCode = (hashCode * 397) ^ MvModifier;
                return hashCode;
            }
        }
    }
}