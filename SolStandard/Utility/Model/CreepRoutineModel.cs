using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Creeps;

namespace SolStandard.Utility.Model
{
    public enum Routine
    {
        None,
        BasicAttack,
        StationaryAttack,
        Wander,
        Summon,
        TreasureHunter,
        TriggerHappy,
        Defender,
        Glutton,
        Prey,
        Kingslayer,
        Wait
    }

    public class CreepRoutineModel
    {
        //Tiled Property Names
        private const string ClassProp = "Class";
        private const string CommanderProp = "Commander";
        private const string IndependentProp = "Independent";
        private const string ItemsProp = "Items";
        private const string TeamProp = "Team";
        private const string GoldProp = "gold";
        private const string CreepPoolProp = "creepPool";
        private const string FallbackRoutineProp = "fallback_routine";
        private const string RoutineBasicAttackProp = "routine_basicAttack";
        private const string RoutineStationaryAttackProp = "routine_stationaryAttack";
        private const string RoutineSummonProp = "routine_summon";
        private const string RoutineSummonClassProp = "routine_summon.class";
        private const string RoutineWanderProp = "routine_wander";
        private const string RoutineTreasureHunterProp = "routine_treasureHunter";
        private const string RoutineTriggerHappyProp = "routine_triggerHappy";
        private const string RoutineDefenderProp = "routine_defender";
        private const string RoutineGluttonProp = "routine_glutton";
        private const string RoutinePreyProp = "routine_prey";
        private const string RoutineKingslayerProp = "routine_kingslayer";
        private const string RoutineWaitProp = "routine_wait";

        public Role CreepClass { get; }
        public string CreepPool { get; }
        private readonly bool isCommander;
        private readonly string items;
        private readonly Team team;
        private readonly Routine fallbackRoutine;
        private readonly bool routineBasicAttack;
        private readonly bool routineStationaryAttack;
        private readonly bool routineSummon;
        private readonly string routineSummonClass;
        private readonly bool routineWander;
        private readonly bool routineTreasureHunter;
        private readonly bool routineTriggerHappy;
        private readonly bool routineDefender;
        private readonly bool routineGlutton;
        private readonly bool routinePrey;
        private readonly bool routineKingslayer;
        private readonly bool routineWait;
        public int StartingGold { get; }
        public bool IsIndependent { get; }

        private CreepRoutineModel(Role creepClass, bool isCommander, bool isIndependent, string items, Team team,
            string creepPool, int startingGold, Routine fallbackRoutine, bool routineBasicAttack,
            bool routineStationaryAttack, bool routineSummon, string routineSummonClass, bool routineWander,
            bool routineTreasureHunter, bool routineTriggerHappy, bool routineDefender, bool routineGlutton,
            bool routinePrey, bool routineKingslayer, bool routineWait)
        {
            CreepClass = creepClass;
            CreepPool = creepPool;
            this.isCommander = isCommander;
            IsIndependent = isIndependent;
            this.items = items;
            this.team = team;
            StartingGold = startingGold;
            this.fallbackRoutine = fallbackRoutine;
            this.routineBasicAttack = routineBasicAttack;
            this.routineStationaryAttack = routineStationaryAttack;
            this.routineSummon = routineSummon;
            this.routineSummonClass = routineSummonClass;
            this.routineWander = routineWander;
            this.routineTreasureHunter = routineTreasureHunter;
            this.routineTriggerHappy = routineTriggerHappy;
            this.routineDefender = routineDefender;
            this.routineGlutton = routineGlutton;
            this.routinePrey = routinePrey;
            this.routineKingslayer = routineKingslayer;
            this.routineWait = routineWait;
        }

        public List<UnitAction> Actions => GenerateCreepRoutinesFromProperties(EntityProperties);


        public IRoutine FallbackRoutine =>
            GenerateRoutine(GetRoutineByName(EntityProperties[FallbackRoutineProp]), EntityProperties) as
                IRoutine;

        private static List<UnitAction> GenerateCreepRoutinesFromProperties(
            IReadOnlyDictionary<string, string> creepProperties
        )
        {
            List<string> enabledRoutines = (from valuePair in creepProperties
                where GetRoutineByName(valuePair.Key) != Routine.None
                where Convert.ToBoolean(valuePair.Value)
                select valuePair.Key).ToList();

            var actions = new List<UnitAction>();

            foreach (string routineName in enabledRoutines)
            {
                actions.Add(GenerateRoutine(GetRoutineByName(routineName), creepProperties));
            }

            return actions;
        }

        private static UnitAction GenerateRoutine(Routine routine, IReadOnlyDictionary<string, string> creepProperties)
        {
            return GenerateRoutine(
                routine,
                Convert.ToBoolean(creepProperties[IndependentProp]),
                creepProperties[RoutineSummonClassProp]
            );
        }

        private static UnitAction GenerateRoutine(Routine routine, bool isIndependent, string summonName)
        {
            switch (routine)
            {
                case Routine.BasicAttack:
                    return new BasicAttackRoutine(isIndependent);
                case Routine.StationaryAttack:
                    return new StationaryAttackRoutine(isIndependent);
                case Routine.Wander:
                    return new WanderRoutine();
                case Routine.Summon:
                    CreepEntity creepToSummon = FindSummonByName(summonName);
                    return new SummoningRoutine(creepToSummon.Model);
                case Routine.TreasureHunter:
                    return new TreasureHunterRoutine();
                case Routine.TriggerHappy:
                    return new TriggerHappyRoutine();
                case Routine.Defender:
                    return new DefenderRoutine();
                case Routine.Glutton:
                    return new GluttonRoutine();
                case Routine.Prey:
                    return new PreyRoutine(isIndependent);
                case Routine.Kingslayer:
                    return new KingslayerRoutine(isIndependent);
                case Routine.Wait:
                    return new WaitRoutine();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static CreepEntity FindSummonByName(string summonName)
        {
            return GlobalContext.WorldContext.MapContainer.MapSummons.Find(creep => creep.Name == summonName);
        }

        public static CreepRoutineModel GetModelForCreep(Dictionary<string, string> unitProperties)
        {
            return new CreepRoutineModel(
                GetRoleByName(unitProperties[ClassProp]),
                Convert.ToBoolean(unitProperties[CommanderProp]),
                Convert.ToBoolean(unitProperties[IndependentProp]),
                unitProperties[ItemsProp],
                GetTeamByName(unitProperties[TeamProp]),
                unitProperties[CreepPoolProp],
                Convert.ToInt32(unitProperties[GoldProp]),
                GetRoutineByName(unitProperties[FallbackRoutineProp]),
                Convert.ToBoolean(unitProperties[RoutineBasicAttackProp]),
                Convert.ToBoolean(unitProperties[RoutineStationaryAttackProp]),
                Convert.ToBoolean(unitProperties[RoutineSummonProp]),
                unitProperties[RoutineSummonClassProp],
                Convert.ToBoolean(unitProperties[RoutineWanderProp]),
                Convert.ToBoolean(unitProperties[RoutineTreasureHunterProp]),
                Convert.ToBoolean(unitProperties[RoutineTriggerHappyProp]),
                Convert.ToBoolean(unitProperties[RoutineDefenderProp]),
                Convert.ToBoolean(unitProperties[RoutineGluttonProp]),
                Convert.ToBoolean(unitProperties[RoutinePreyProp]),
                Convert.ToBoolean(unitProperties[RoutineKingslayerProp]),
                Convert.ToBoolean(unitProperties[RoutineWaitProp])
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

        private static Routine GetRoutineByName(string routineName)
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;

            if (RoutineBasicAttackProp.EndsWith(routineName, true, invariantCulture)) return Routine.BasicAttack;
            if (RoutineStationaryAttackProp.EndsWith(routineName, true, invariantCulture))
                return Routine.StationaryAttack;
            if (RoutineSummonProp.EndsWith(routineName, true, invariantCulture)) return Routine.Summon;
            if (RoutineWanderProp.EndsWith(routineName, true, invariantCulture)) return Routine.Wander;
            if (RoutineTreasureHunterProp.EndsWith(routineName, true, invariantCulture)) return Routine.TreasureHunter;
            if (RoutineTriggerHappyProp.EndsWith(routineName, true, invariantCulture)) return Routine.TriggerHappy;
            if (RoutineDefenderProp.EndsWith(routineName, true, invariantCulture)) return Routine.Defender;
            if (RoutineGluttonProp.EndsWith(routineName, true, invariantCulture)) return Routine.Glutton;
            if (RoutinePreyProp.EndsWith(routineName, true, invariantCulture)) return Routine.Prey;
            if (RoutineKingslayerProp.EndsWith(routineName, true, invariantCulture)) return Routine.Kingslayer;
            if (RoutineWaitProp.EndsWith(routineName, true, invariantCulture)) return Routine.Wait;

            return Routine.None;
        }

        public Dictionary<string, string> EntityProperties =>
            new Dictionary<string, string>
            {
                {ClassProp, CreepClass.ToString()},
                {CommanderProp, isCommander.ToString()},
                {IndependentProp, IsIndependent.ToString()},
                {ItemsProp, items},
                {TeamProp, team.ToString()},
                {CreepPoolProp, CreepPool},
                {GoldProp, StartingGold.ToString()},
                {FallbackRoutineProp, $"routine_{fallbackRoutine.ToString()}"},
                {RoutineBasicAttackProp, routineBasicAttack.ToString()},
                {RoutineStationaryAttackProp, routineStationaryAttack.ToString()},
                {RoutineSummonProp, routineSummon.ToString()},
                {RoutineSummonClassProp, routineSummonClass},
                {RoutineWanderProp, routineWander.ToString()},
                {RoutineTreasureHunterProp, routineTreasureHunter.ToString()},
                {RoutineTriggerHappyProp, routineTriggerHappy.ToString()},
                {RoutineDefenderProp, routineDefender.ToString()},
                {RoutineGluttonProp, routineGlutton.ToString()},
                {RoutinePreyProp, routinePrey.ToString()},
                {RoutineKingslayerProp, routineKingslayer.ToString()},
                {RoutineWaitProp, routineWait.ToString()}
            };
    }
}