using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Archer;
using SolStandard.Entity.Unit.Actions.Bard;
using SolStandard.Entity.Unit.Actions.Champion;
using SolStandard.Entity.Unit.Actions.Cleric;
using SolStandard.Entity.Unit.Actions.Creeps;
using SolStandard.Entity.Unit.Actions.Duelist;
using SolStandard.Entity.Unit.Actions.Lancer;
using SolStandard.Entity.Unit.Actions.Mage;
using SolStandard.Entity.Unit.Actions.Marauder;
using SolStandard.Entity.Unit.Actions.Paladin;
using SolStandard.Entity.Unit.Actions.Pugilist;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Model;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public static class UnitGenerator
    {
        private const string RoutinePrefix = "routine_";
        private const string RoutinePropertyDelimiter = ".";


        public static List<GameUnit> GenerateUnitsFromMap(IEnumerable<UnitEntity> units, List<IItem> loot)
        {
            List<GameUnit> unitsFromMap = new List<GameUnit>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (UnitEntity unit in units)
            {
                if (unit == null) continue;

                Team unitTeam = (Team) Enum.Parse(typeof(Team), unit.TiledProperties["Team"]);
                Role role = (Role) Enum.Parse(typeof(Role), unit.TiledProperties["Class"]);
                bool commander = Convert.ToBoolean(unit.TiledProperties["Commander"]);

                GameUnit unitToBuild = BuildUnitFromProperties(unit.Name, unitTeam, role, commander, unit, loot,
                    unit.TiledProperties);
                unitsFromMap.Add(unitToBuild);
            }

            return unitsFromMap;
        }

        private static GameUnit BuildUnitFromProperties(string id, Team unitTeam, Role unitJobClass, bool isCommander,
            UnitEntity mapEntity, List<IItem> loot, Dictionary<string, string> tiledProperties)
        {
            GameUnit generatedUnit;

            if (unitTeam == Team.Creep)
            {
                generatedUnit = GenerateCreep(unitJobClass, unitTeam, id, isCommander, mapEntity as CreepEntity,
                    tiledProperties);
                PopulateUnitInventory(mapEntity, loot, generatedUnit);
                AssignStartingGold(generatedUnit);
            }
            else
            {
                generatedUnit = GenerateUnit(unitJobClass, unitTeam, id, isCommander, mapEntity);
            }

            return generatedUnit;
        }

        private static void AssignStartingGold(GameUnit generatedUnit)
        {
            switch (generatedUnit.Role)
            {
                case Role.Slime:
                    generatedUnit.CurrentGold += 3 + GameDriver.Random.Next(5);
                    break;
                case Role.Troll:
                    generatedUnit.CurrentGold += 14 + GameDriver.Random.Next(5);
                    break;
                case Role.Orc:
                    generatedUnit.CurrentGold += 7 + GameDriver.Random.Next(8);
                    break;
                case Role.Necromancer:
                    generatedUnit.CurrentGold += 14 + GameDriver.Random.Next(8);
                    break;
                case Role.Skeleton:
                    generatedUnit.CurrentGold += 5 + GameDriver.Random.Next(8);
                    break;
                case Role.Goblin:
                    generatedUnit.CurrentGold += 5 + GameDriver.Random.Next(8);
                    break;
                case Role.Rat:
                    generatedUnit.CurrentGold += 3 + GameDriver.Random.Next(5);
                    break;
                case Role.Bat:
                    generatedUnit.CurrentGold += 5 + GameDriver.Random.Next(8);
                    break;
                case Role.Spider:
                    generatedUnit.CurrentGold += 3 + GameDriver.Random.Next(5);
                    break;
            }
        }

        private static void PopulateUnitInventory(MapEntity mapEntity, List<IItem> loot, GameUnit unit)
        {
            string itemNameProp = mapEntity.TiledProperties["Items"];
            if (itemNameProp == string.Empty) return;
            string[] unitInventory = itemNameProp.Split('|');

            foreach (string itemName in unitInventory)
            {
                IItem unitItem = loot.Find(item => item.Name == itemName);

                unit.AddItemToInventory(unitItem);
            }
        }

        //UNITS

        private static UnitStatistics SelectArcherStats()
        {
            return new UnitStatistics(hp: 7, armor: 5, atk: 6, ret: 4, luck: 1, mv: 5, atkRange: new[] {2});
        }

        private static UnitStatistics SelectChampionStats()
        {
            return new UnitStatistics(hp: 7, armor: 9, atk: 5, ret: 5, luck: 1, mv: 6, atkRange: new[] {1});
        }

        private static UnitStatistics SelectMageStats()
        {
            return new UnitStatistics(hp: 6, armor: 4, atk: 6, ret: 3, luck: 1, mv: 5, atkRange: new[] {1, 2});
        }

        private static UnitStatistics SelectLancerStats()
        {
            return new UnitStatistics(hp: 8, armor: 5, atk: 6, ret: 4, luck: 1, mv: 6, atkRange: new[] {1});
        }

        private static UnitStatistics SelectBardStats()
        {
            return new UnitStatistics(hp: 8, armor: 3, atk: 3, ret: 3, luck: 2, mv: 5, atkRange: new[] {1, 2});
        }

        private static UnitStatistics SelectPugilistStats()
        {
            return new UnitStatistics(hp: 9, armor: 4, atk: 7, ret: 4, luck: 0, mv: 6, atkRange: new[] {1});
        }

        private static UnitStatistics SelectDuelistStats()
        {
            return new UnitStatistics(hp: 8, armor: 5, atk: 5, ret: 4, luck: 1, mv: 6, atkRange: new[] {1});
        }

        private static UnitStatistics SelectClericStats()
        {
            return new UnitStatistics(hp: 6, armor: 6, atk: 0, ret: 0, luck: 4, mv: 6, atkRange: new[] {1, 2});
        }

        private static UnitStatistics SelectMarauderStats()
        {
            return new UnitStatistics(hp: 18, armor: 0, atk: 5, ret: 5, luck: 0, mv: 6, atkRange: new[] {1});
        }

        private static UnitStatistics SelectPaladinStats()
        {
            return new UnitStatistics(hp: 8, armor: 8, atk: 5, ret: 6, luck: 1, mv: 6, atkRange: new[] {1});
        }

        //CREEPS

        private static UnitStatistics SelectSlimeStats()
        {
            return new UnitStatistics(hp: 7, armor: 0, atk: 3, ret: 3, luck: 0, mv: 3, atkRange: new[] {1});
        }

        private static UnitStatistics SelectTrollStats()
        {
            return new UnitStatistics(hp: 20, armor: 3, atk: 6, ret: 4, luck: 2, mv: 4, atkRange: new[] {1});
        }

        private static UnitStatistics SelectOrcStats()
        {
            return new UnitStatistics(hp: 15, armor: 0, atk: 5, ret: 4, luck: 0, mv: 4, atkRange: new[] {1});
        }

        private static UnitStatistics SelectNecromancerStats()
        {
            return new UnitStatistics(hp: 15, armor: 5, atk: 6, ret: 5, luck: 1, mv: 4, atkRange: new[] {1, 2});
        }

        private static UnitStatistics SelectSkeletonStats()
        {
            return new UnitStatistics(hp: 12, armor: 0, atk: 4, ret: 4, luck: 0, mv: 4, atkRange: new[] {1});
        }

        private static UnitStatistics SelectGoblinStats()
        {
            return new UnitStatistics(hp: 10, armor: 2, atk: 4, ret: 4, luck: 1, mv: 4, atkRange: new[] {1});
        }

        private static UnitStatistics SelectRatStats()
        {
            return new UnitStatistics(hp: 9, armor: 0, atk: 3, ret: 3, luck: 0, mv: 5, atkRange: new[] {1});
        }

        private static UnitStatistics SelectBatStats()
        {
            return new UnitStatistics(hp: 10, armor: 0, atk: 4, ret: 4, luck: 1, mv: 5, atkRange: new[] {1});
        }

        private static UnitStatistics SelectSpiderStats()
        {
            return new UnitStatistics(hp: 4, armor: 4, atk: 4, ret: 4, luck: 0, mv: 5, atkRange: new[] {1});
        }

        private static List<UnitAction> SelectArcherSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Draw(3, 1),
                new PoisonArrow(2, 4),
                new HuntingTrap(6, 1),
                new Sprint(3),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> SelectChampionSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Bloodthirst(1),
                new Challenge(2),
                new Tackle(),
                new Shove(),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> SelectMageSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Ignite(3, 3),
                new Inferno(3, 2),
                new Atrophy(2, 1),
                new Replace(),
                new Sprint(3),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> SelectLancerSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new LeapStrike(3),
                new Cripple(2, 2),
                new Execute(3),
                new Sprint(3),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> SelectBardSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Crescendo(2, 1),
                new Accelerando(2, 1),
                new Capriccio(2, 1),
                new Sprint(3),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> SelectPugilistSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Uppercut(),
                new PressurePoint(60),
                new Meditate(),
                new Sprint(3),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> SelectDuelistSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new PhaseStrike(),
                new Bloodthirst(2),
                new Shift(1),
                new Guard(3),
                new Focus(2)
            };
        }

        private static List<UnitAction> SelectClericSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Recover(3),
                new Bulwark(2, 2),
                new Cleanse(),
                new Replace(),
                new Sprint(3),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> SelectMarauderSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Guillotine(),
                new Rage(3, 3),
                new Brace(2),
                new Shove(),
                new Wait()
            };
        }

        private static List<UnitAction> SelectPaladinSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Rampart(3, 2),
                new Stun(1),
                new Replace(),
                new Shove(),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> GenerateCreepRoutinesFromProperties(
            IReadOnlyDictionary<string, string> creepProperties
        )
        {
            List<string> enabledRoutines = (from valuePair in creepProperties
                where valuePair.Key.StartsWith(RoutinePrefix) && !valuePair.Key.Contains(RoutinePropertyDelimiter)
                where Convert.ToBoolean(valuePair.Value)
                select valuePair.Key).ToList();

            List<UnitAction> actions = new List<UnitAction>();

            foreach (string routineName in enabledRoutines)
            {
                actions.Add(GenerateRoutine(CreepModel.GetRoutineByName(routineName), creepProperties));
            }

            return actions;
        }

        private static UnitAction GenerateRoutine(Routine routine, IReadOnlyDictionary<string, string> creepProperties)
        {
            bool isIndependent = Convert.ToBoolean(creepProperties["Independent"]);

            switch (routine)
            {
                case Routine.BasicAttack:
                    return new BasicAttackRoutine(isIndependent);
                case Routine.Wander:
                    return (new WanderRoutine());
                case Routine.Summon:
                    return new SummoningRoutine(GetRoleByName(
                        creepProperties[RoutinePrefix + "summon" + RoutinePropertyDelimiter + "class"]
                    ));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Role GetRoleByName(string roleName)
        {
            return (Role) Enum.Parse(typeof(Role), roleName);
        }


        private static ITexture2D FindSmallPortrait(string unitTeam, string unitJobClass, List<ITexture2D> portraits)
        {
            return portraits.Find(texture =>
                texture.MonoGameTexture.Name.Contains(unitTeam) && texture.MonoGameTexture.Name.Contains(unitJobClass));
        }

        public static ITexture2D GetUnitPortrait(Role role, Team team)
        {
            return AssetManager.SmallPortraitTextures
                .Find(texture =>
                    texture.MonoGameTexture.Name.Contains(team.ToString()) &&
                    texture.MonoGameTexture.Name.Contains(role.ToString())
                );
        }

        public static CreepUnit GenerateAdHocCreep(Role role, Dictionary<string, string> entityProperties)
        {
            string unitName = NameGenerator.GenerateUnitName(role);
            
            CreepEntity generatedEntity = GenerateCreepEntity(unitName, "Creep", role, Team.Creep, false,
                AssetManager.UnitSprites, Vector2.Zero, entityProperties);

            return GenerateCreep(role, Team.Creep, unitName, false, generatedEntity, entityProperties);
        }

        public static GameUnit GenerateAdHocUnit(Role role, Team team, bool isCommander)
        {
            string unitName = NameGenerator.GenerateUnitName(role);

            if (team == Team.Creep)
            {
                throw new Exception("Cannot create creep as if it were a player unit!");
            }

            Dictionary<string, string> tiledProperties = new Dictionary<string, string>();

            UnitEntity generatedEntity = GenerateUnitEntity(unitName, "Unit", role, team, isCommander,
                AssetManager.UnitSprites, Vector2.Zero, tiledProperties);

            return GenerateUnit(role, team, unitName, isCommander, generatedEntity);
        }

        private static CreepUnit GenerateCreep(Role role, Team team, string unitName, bool isCommander,
            CreepEntity entity, IReadOnlyDictionary<string, string> tiledProperties)
        {
            ITexture2D portrait =
                FindSmallPortrait(team.ToString(), role.ToString(), AssetManager.SmallPortraitTextures);

            UnitStatistics unitStatistics;
            List<UnitAction> unitActions;

            switch (role)
            {
                case Role.Slime:
                    unitStatistics = SelectSlimeStats();
                    unitActions = GenerateCreepRoutinesFromProperties(tiledProperties);
                    break;
                case Role.Troll:
                    unitStatistics = SelectTrollStats();
                    unitActions = GenerateCreepRoutinesFromProperties(tiledProperties);
                    break;
                case Role.Orc:
                    unitStatistics = SelectOrcStats();
                    unitActions = GenerateCreepRoutinesFromProperties(tiledProperties);
                    break;
                case Role.Necromancer:
                    unitStatistics = SelectNecromancerStats();
                    unitActions = GenerateCreepRoutinesFromProperties(tiledProperties);
                    break;
                case Role.Skeleton:
                    unitStatistics = SelectSkeletonStats();
                    unitActions = GenerateCreepRoutinesFromProperties(tiledProperties);
                    break;
                case Role.Goblin:
                    unitStatistics = SelectGoblinStats();
                    unitActions = GenerateCreepRoutinesFromProperties(tiledProperties);
                    break;
                case Role.Rat:
                    unitStatistics = SelectRatStats();
                    unitActions = GenerateCreepRoutinesFromProperties(tiledProperties);
                    break;
                case Role.Bat:
                    unitStatistics = SelectBatStats();
                    unitActions = GenerateCreepRoutinesFromProperties(tiledProperties);
                    break;
                case Role.Spider:
                    unitStatistics = SelectSpiderStats();
                    unitActions = GenerateCreepRoutinesFromProperties(tiledProperties);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("role", role, null);
            }

            UnitAction fallbackRoutine = GenerateRoutine(
                CreepModel.GetRoutineByName(tiledProperties[CreepModel.FallbackRoutineProp]),
                tiledProperties
            );

            return new CreepUnit(unitName, team, role, entity, unitStatistics, portrait, unitActions, isCommander,
                fallbackRoutine as IRoutine);
        }

        private static GameUnit GenerateUnit(Role role, Team team, string unitName, bool isCommander, UnitEntity entity)
        {
            ITexture2D portrait =
                FindSmallPortrait(team.ToString(), role.ToString(), AssetManager.SmallPortraitTextures);

            UnitStatistics unitStatistics;
            List<UnitAction> unitActions;

            switch (role)
            {
                case Role.Archer:
                    unitStatistics = SelectArcherStats();
                    unitActions = SelectArcherSkills();
                    break;
                case Role.Champion:
                    unitStatistics = SelectChampionStats();
                    unitActions = SelectChampionSkills();
                    break;
                case Role.Mage:
                    unitStatistics = SelectMageStats();
                    unitActions = SelectMageSkills();
                    break;
                case Role.Lancer:
                    unitStatistics = SelectLancerStats();
                    unitActions = SelectLancerSkills();
                    break;
                case Role.Bard:
                    unitStatistics = SelectBardStats();
                    unitActions = SelectBardSkills();
                    break;
                case Role.Pugilist:
                    unitStatistics = SelectPugilistStats();
                    unitActions = SelectPugilistSkills();
                    break;
                case Role.Duelist:
                    unitStatistics = SelectDuelistStats();
                    unitActions = SelectDuelistSkills();
                    break;
                case Role.Cleric:
                    unitStatistics = SelectClericStats();
                    unitActions = SelectClericSkills();
                    break;
                case Role.Marauder:
                    unitStatistics = SelectMarauderStats();
                    unitActions = SelectMarauderSkills();
                    break;
                case Role.Paladin:
                    unitStatistics = SelectPaladinStats();
                    unitActions = SelectPaladinSkills();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("role", role, null);
            }

            return new GameUnit(unitName, team, role, entity, unitStatistics, portrait, unitActions, isCommander);
        }

        public static UnitEntity GenerateMapEntity(string name, string type, Role role, Team team, bool isCommander,
            List<ITexture2D> unitSprites, Vector2 mapCoordinates, Dictionary<string, string> unitProperties)
        {
            return team == Team.Creep
                ? GenerateCreepEntity(name, type, role, team, isCommander, unitSprites, mapCoordinates, unitProperties)
                : GenerateUnitEntity(name, type, role, team, isCommander, unitSprites, mapCoordinates, unitProperties);
        }

        private static CreepEntity GenerateCreepEntity(string name, string type, Role role, Team team, bool isCommander,
            List<ITexture2D> unitSprites, Vector2 mapCoordinates, Dictionary<string, string> unitProperties)
        {
            UnitSpriteSheet animatedSpriteSheet = GenerateUnitSpriteSheet(role, team, unitSprites);

            CreepEntity creepEntity = new CreepEntity(name, type, animatedSpriteSheet, mapCoordinates, isCommander,
                unitProperties);

            return creepEntity;
        }

        private static UnitEntity GenerateUnitEntity(string name, string type, Role role, Team team, bool isCommander,
            List<ITexture2D> unitSprites, Vector2 mapCoordinates, Dictionary<string, string> unitProperties)
        {
            UnitSpriteSheet animatedSpriteSheet = GenerateUnitSpriteSheet(role, team, unitSprites);

            UnitEntity unitEntity = new UnitEntity(name, type, animatedSpriteSheet, mapCoordinates, isCommander,
                unitProperties);

            return unitEntity;
        }

        private static UnitSpriteSheet GenerateUnitSpriteSheet(Role role, Team team, List<ITexture2D> unitSprites)
        {
            ITexture2D unitSprite = FetchUnitGraphic(team.ToString(), role.ToString(), unitSprites);

            Vector2 unitScale = new Vector2(unitSprite.Width) / 2.5f;
            const int unitAnimationFrames = 4;
            const int unitAnimationDelay = 12;

            UnitSpriteSheet animatedSpriteSheet = new UnitSpriteSheet(
                unitSprite,
                unitSprite.Width / unitAnimationFrames,
                unitScale,
                unitAnimationDelay,
                false,
                Color.White
            );
            return animatedSpriteSheet;
        }

        private static ITexture2D FetchUnitGraphic(string unitTeam, string role, List<ITexture2D> unitSprites)
        {
            string unitTeamAndClass = unitTeam + role;
            return unitSprites.Find(texture => texture.Name.Contains(unitTeamAndClass));
        }
    }
}