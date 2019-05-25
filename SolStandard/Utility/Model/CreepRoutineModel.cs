using System;
using System.Collections.Generic;
using System.Globalization;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Creeps;

namespace SolStandard.Utility.Model
{
    public enum Routine
    {
        None,
        BasicAttack,
        Wander,
        Summon,
        TreasureHunter,
        TriggerHappy,
        Defender,
        Glutton
    }

    public class CreepRoutineModel
    {
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
        public const string RoutineTriggerHappyProp = "routine_triggerHappy";
        public const string RoutineDefenderProp = "routine_defender";
        public const string RoutineGluttonProp = "routine_glutton";

        public Role CreepClass { get; private set; }
        private readonly bool isCommander;
        private readonly bool isIndependent;
        private readonly string items;
        private readonly Team team;
        private readonly Routine fallbackRoutine;
        private readonly bool routineBasicAttack;
        private readonly bool routineSummon;
        private readonly string routineSummonClass;
        private readonly bool routineWander;
        private readonly bool routineTreasureHunter;
        private readonly bool routineTriggerHappy;
        private readonly bool routineDefender;
        private readonly bool routineGlutton;

        private CreepRoutineModel(Role creepClass, bool isCommander, bool isIndependent, string items, Team team,
            Routine fallbackRoutine, bool routineBasicAttack, bool routineSummon, string routineSummonClass,
            bool routineWander, bool routineTreasureHunter, bool routineTriggerHappy, bool routineDefender,
            bool routineGlutton)
        {
            CreepClass = creepClass;
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
            this.routineTriggerHappy = routineTriggerHappy;
            this.routineDefender = routineDefender;
            this.routineGlutton = routineGlutton;
        }

        public static UnitAction GenerateRoutine(Routine routine, IReadOnlyDictionary<string, string> creepProperties)
        {
            bool isIndependent = Convert.ToBoolean(creepProperties[IndependentProp]);

            switch (routine)
            {
                case Routine.BasicAttack:
                    return new BasicAttackRoutine(isIndependent);
                case Routine.Wander:
                    return new WanderRoutine();
                case Routine.Summon:
                    CreepEntity creepToSummon = FindSummonByName(creepProperties[RoutineSummonClassProp]);
                    return new SummoningRoutine(GetModelForCreep(creepToSummon));
                case Routine.TreasureHunter:
                    return new TreasureHunterRoutine();
                case Routine.TriggerHappy:
                    return new TriggerHappyRoutine();
                case Routine.Defender:
                    return new DefenderRoutine();
                case Routine.Glutton:
                    return new GluttonRoutine();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static CreepEntity FindSummonByName(string summonName)
        {
            return GameContext.GameMapContext.MapContainer.MapSummons.Find(creep => creep.Name == summonName);
        }

        private static CreepRoutineModel GetModelForCreep(CreepEntity creepEntity)
        {
            Dictionary<string, string> creepProps = creepEntity.TiledProperties;
            return new CreepRoutineModel(
                GetRoleByName(creepProps[ClassProp]),
                Convert.ToBoolean(creepProps[CommanderProp]),
                Convert.ToBoolean(creepProps[IndependentProp]),
                creepProps[ItemsProp],
                GetTeamByName(creepProps[TeamProp]),
                GetRoutineByName(creepProps[FallbackRoutineProp]),
                Convert.ToBoolean(creepProps[RoutineBasicAttackProp]),
                Convert.ToBoolean(creepProps[RoutineSummonProp]),
                creepProps[RoutineSummonClassProp],
                Convert.ToBoolean(creepProps[RoutineWanderProp]),
                Convert.ToBoolean(creepProps[RoutineTreasureHunterProp]),
                Convert.ToBoolean(creepProps[RoutineTriggerHappyProp]),
                Convert.ToBoolean(creepProps[RoutineDefenderProp]),
                Convert.ToBoolean(creepProps[RoutineGluttonProp])
            );
        }

        private static Team GetTeamByName(string teamName)
        {
            return (Team) Enum.Parse(typeof(Team), teamName);
        }

        private static Role GetRoleByName(string roleName)
        {
            return (Role) Enum.Parse(typeof(Role), roleName);
        }

        public static Routine GetRoutineByName(string routineName)
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            if (RoutineBasicAttackProp.EndsWith(routineName, true, invariantCulture)) return Routine.BasicAttack;
            if (RoutineSummonProp.EndsWith(routineName, true, invariantCulture)) return Routine.Summon;
            if (RoutineWanderProp.EndsWith(routineName, true, invariantCulture)) return Routine.Wander;
            if (RoutineTreasureHunterProp.EndsWith(routineName, true, invariantCulture)) return Routine.TreasureHunter;
            if (RoutineTriggerHappyProp.EndsWith(routineName, true, invariantCulture)) return Routine.TriggerHappy;
            if (RoutineDefenderProp.EndsWith(routineName, true, invariantCulture)) return Routine.Defender;
            if (RoutineGluttonProp.EndsWith(routineName, true, invariantCulture)) return Routine.Glutton;

            return Routine.None;
        }

        public Dictionary<string, string> EntityProperties
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {ClassProp, CreepClass.ToString()},
                    {CommanderProp, isCommander.ToString()},
                    {IndependentProp, isIndependent.ToString()},
                    {ItemsProp, items},
                    {TeamProp, team.ToString()},
                    {FallbackRoutineProp, string.Format("routine_{0}", fallbackRoutine.ToString())},
                    {RoutineBasicAttackProp, routineBasicAttack.ToString()},
                    {RoutineSummonProp, routineSummon.ToString()},
                    {RoutineSummonClassProp, routineSummonClass.ToString()},
                    {RoutineWanderProp, routineWander.ToString()},
                    {RoutineTreasureHunterProp, routineTreasureHunter.ToString()},
                    {RoutineTriggerHappyProp, routineTriggerHappy.ToString()},
                    {RoutineDefenderProp, routineDefender.ToString()},
                    {RoutineGluttonProp, routineGlutton.ToString()}
                };
            }
        }
    }
}