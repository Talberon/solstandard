using System;
using System.IO;

namespace SolStandard.Entity.Unit
{
    public enum UnitParameters
    {
        Hp,
        Atk,
        Def,
        Sp,
        Ap,
        Mv,
        Rng
    }

    public class UnitStatistics
    {
        private int hp;
        private int atk;
        private int def;
        private int sp;
        private int ap;
        private int mv;
        private int[] rng;

        private readonly int maxHp;
        private readonly int baseAtk;
        private readonly int baseDef;
        private readonly int maxSp;
        private readonly int maxAp;
        private readonly int maxMv;
        private readonly int[] baseRng;


        public UnitStatistics(int hp, int atk, int def, int sp, int ap, int mv, int[] rng)
        {
            Hp = hp;
            Atk = atk;
            Def = def;
            Sp = sp;
            Ap = ap;
            Mv = mv;
            Rng = rng;

            maxHp = hp;
            baseAtk = atk;
            baseDef = def;
            maxSp = sp;
            maxAp = ap;
            maxMv = mv;
            baseRng = rng;
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

        public int MaxAp
        {
            get { return maxAp; }
        }

        public int MaxMv
        {
            get { return maxMv; }
        }

        public int[] BaseRng
        {
            get { return baseRng; }
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

        public int Ap
        {
            get { return ap; }
            set { ap = value; }
        }

        public int Mv
        {
            get { return mv; }
            set { mv = value; }
        }

        public int[] Rng
        {
            get { return rng; }
            set { rng = value; }
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
            output += "AP: " + Ap.ToString() + "/" + maxAp;
            output += Environment.NewLine;
            output += "MV: " + Mv.ToString() + "/" + maxMv;
            output += Environment.NewLine;
            output += string.Format("RNG: [{0}]/[{1}]", string.Join(",", Rng), string.Join(",", baseRng));

            return output;

        }
    }
}