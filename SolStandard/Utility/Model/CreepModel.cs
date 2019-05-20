using System.Collections.Generic;
using System.Globalization;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Model
{
    public enum Routine
    {
        None,
        BasicAttack,
        Wander,
        Summon,
        TreasureHunter
    }

    public class CreepModel
    {
        public static Routine GetRoutineByName(string routineName)
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            if (RoutineBasicAttackProp.EndsWith(routineName, true, invariantCulture)) return Routine.BasicAttack;
            if (RoutineSummonProp.EndsWith(routineName, true, invariantCulture)) return Routine.Summon;
            if (RoutineWanderProp.EndsWith(routineName, true, invariantCulture)) return Routine.Wander;
            if (RoutineTreasureHunterProp.EndsWith(routineName, true, invariantCulture)) return Routine.TreasureHunter;

            return Routine.None;
        }

        //Tiled Property Names
        public const string ClassProp = "Class";
        public const string CommanderProp = "Commander";
        public const string IndependentProp = "Independent";
        public const string ItemsProp = "Items";
        public const string TeamProp = "Team";
        public const string FallbackRoutineProp = "fallback_routine";
        public const string RoutineBasicAttackProp = "routine_basicAttack";
        public const string RoutineSummonProp = "routine_summon";
        public const string RoutineSummonClassProp = "routine_summon.class";
        public const string RoutineWanderProp = "routine_wander";
        public const string RoutineTreasureHunterProp = "routine_treasureHunter";

        private readonly Role creepClass;
        private readonly bool isCommander;
        private readonly bool isIndependent;
        private readonly string items;
        private readonly Team team;
        private readonly Routine fallbackRoutine;
        private readonly bool routineBasicAttack;
        private readonly bool routineSummon;
        private readonly Role routineSummonClass;
        private readonly bool routineWander;
        private readonly bool routineTreasureHunter;

        public CreepModel(Role creepClass, bool isCommander, bool isIndependent, string items, Team team,
            Routine fallbackRoutine, bool routineBasicAttack, bool routineSummon, Role routineSummonClass,
            bool routineWander, bool routineTreasureHunter)
        {
            this.creepClass = creepClass;
            this.isCommander = isCommander;
            this.isIndependent = isIndependent;
            this.items = items;
            this.team = team;
            this.fallbackRoutine = fallbackRoutine;
            this.routineBasicAttack = routineBasicAttack;
            this.routineSummon = routineSummon;
            this.routineSummonClass = routineSummonClass;
            this.routineWander = routineWander;
            this.routineTreasureHunter = routineTreasureHunter;
        }

        public Dictionary<string, string> EntityProperties
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {ClassProp, creepClass.ToString()},
                    {CommanderProp, isCommander.ToString()},
                    {IndependentProp, isIndependent.ToString()},
                    {ItemsProp, items},
                    {TeamProp, team.ToString()},
                    {FallbackRoutineProp, string.Format("routine_{0}", fallbackRoutine.ToString())},
                    {RoutineBasicAttackProp, routineBasicAttack.ToString()},
                    {RoutineSummonProp, routineSummon.ToString()},
                    {RoutineSummonClassProp, routineSummonClass.ToString()},
                    {RoutineWanderProp, routineWander.ToString()},
                    {RoutineTreasureHunterProp, routineTreasureHunter.ToString()}
                };
            }
        }
    }
}