﻿namespace SolStandard.Entity.Unit
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
        private int rng;

        private readonly int maxHp;
        private readonly int baseAtk;
        private readonly int baseDef;
        private readonly int maxSp;
        private readonly int maxAp;
        private readonly int maxMv;
        private readonly int baseRng;


        public UnitStatistics(int hp, int atk, int def, int sp, int ap, int mv, int rng)
        {
            this.Hp = hp;
            this.Atk = atk;
            this.Def = def;
            this.Sp = sp;
            this.Ap = ap;
            this.Mv = mv;
            this.Rng = rng;

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

        public int BaseRng
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

        public int Rng
        {
            get { return rng; }
            set { rng = value; }
        }
    }
}