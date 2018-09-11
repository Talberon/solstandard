using System;
using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit
{
    public enum StatIcons
    {
        None,
        Hp,
        Atk,
        Def,
        Sp,
        Mv,
        AtkRange,
        BonusHp,
        BonusAtk,
        BonusDef,
        BonusSp,
        BonusMv,
        BonusAtkRange
    }

    public class UnitStatistics
    {
        private int hp;
        private int atk;
        private int def;
        private int sp;
        private int mv;
        private int[] atkRange;

        private readonly int maxHp;
        private readonly int baseAtk;
        private readonly int baseDef;
        private readonly int maxSp;
        private readonly int maxMv;
        private readonly int[] baseAtkRange;


        public UnitStatistics(int hp, int atk, int def, int sp, int mv, int[] atkRange)
        {
            Hp = hp;
            Atk = atk;
            Def = def;
            Sp = sp;
            Mv = mv;
            AtkRange = atkRange;

            maxHp = hp;
            baseAtk = atk;
            baseDef = def;
            maxSp = sp;
            maxMv = mv;
            baseAtkRange = atkRange;
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

        public int MaxSp
        {
            get { return maxSp; }
        }

        public int MaxMv
        {
            get { return maxMv; }
        }

        public int[] BaseAtkRange
        {
            get { return baseAtkRange; }
        }

        public int Hp
        {
            get { return hp; }
            set { hp = value; }
        }

        public int Atk
        {
            get { return atk; }
            set { atk = value; }
        }

        public int Def
        {
            get { return def; }
            set { def = value; }
        }

        public int Sp
        {
            get { return sp; }
            set { sp = value; }
        }

        public int Mv
        {
            get { return mv; }
            set { mv = value; }
        }

        public int[] AtkRange
        {
            get { return atkRange; }
            set { atkRange = value; }
        }


        public static SpriteAtlas GetSpriteAtlas(StatIcons stat)
        {
            return new SpriteAtlas(GameDriver.StatIcons, new Vector2(GameDriver.CellSize), (int) stat);
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
            output += "SP: " + Sp.ToString() + "/" + maxSp;
            output += Environment.NewLine;
            output += "MV: " + Mv.ToString() + "/" + maxMv;
            output += Environment.NewLine;
            output += string.Format("RNG: [{0}]/[{1}]", string.Join(",", AtkRange), string.Join(",", baseAtkRange));

            return output;
        }
    }
}