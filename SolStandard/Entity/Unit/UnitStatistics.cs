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
        AtkRange,
        BonusHp,
        BonusAtk,
        BonusDef,
        BonusMv,
        BonusAtkRange
    }

    public class UnitStatistics
    {
        private readonly int maxHp;
        private readonly int baseAtk;
        private readonly int baseDef;
        private readonly int baseMv;
        private readonly int[] baseAtkRange;


        public UnitStatistics(int hp, int atk, int def, int mv, int[] atkRange)
        {
            Hp = hp;
            Atk = atk;
            Def = def;
            Mv = mv;
            AtkRange = atkRange;

            maxHp = hp;
            baseAtk = atk;
            baseDef = def;
            baseMv = mv;
            baseAtkRange = ArrayDeepCopier<int>.DeepCopyArray(atkRange);
        }

        public int MaxHp
        {
            get { return maxHp; }
        }

        public int BaseAtk
        {
            get { return baseAtk; }
        }

        public int BaseDef
        {
            get { return baseDef; }
        }

        public int BaseMv
        {
            get { return baseMv; }
        }

        public int[] BaseAtkRange
        {
            get { return baseAtkRange; }
        }

        public int Hp { get; set; }

        public int Atk { get; set; }

        public int Def { get; set; }

        public int Mv { get; set; }

        public int[] AtkRange { get; set; }


        public static SpriteAtlas GetSpriteAtlas(StatIcons stat)
        {
            return new SpriteAtlas(AssetManager.StatIcons, new Vector2(GameDriver.CellSize), (int) stat);
        }

        public override string ToString()
        {
            string output = "";

            output += "HP: " + Hp.ToString() + "/" + maxHp;
            output += Environment.NewLine;
            output += "ATK: " + Atk.ToString() + "/" + baseAtk;
            output += Environment.NewLine;
            output += "DEF: " + Def.ToString() + "/" + baseDef;
            output += Environment.NewLine;
            output += "MV: " + Mv.ToString() + "/" + baseMv;
            output += Environment.NewLine;
            output += string.Format("RNG: [{0}]/[{1}]", string.Join(",", AtkRange), string.Join(",", baseAtkRange));

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