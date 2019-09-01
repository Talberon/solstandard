using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Archer;
using SolStandard.Entity.Unit.Actions.Bard;
using SolStandard.Entity.Unit.Actions.Cavalier;
using SolStandard.Entity.Unit.Actions.Champion;
using SolStandard.Entity.Unit.Actions.Cleric;
using SolStandard.Entity.Unit.Actions.Duelist;
using SolStandard.Entity.Unit.Actions.Lancer;
using SolStandard.Entity.Unit.Actions.Mage;
using SolStandard.Entity.Unit.Actions.Marauder;
using SolStandard.Entity.Unit.Actions.Paladin;
using SolStandard.Entity.Unit.Actions.Pugilist;
using SolStandard.Entity.Unit.Actions.Rogue;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Model;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public static class UnitGenerator
    {
        private const int StartingGoldVariance = 5;

        public static List<GameUnit> GenerateUnitsFromMap(IEnumerable<UnitEntity> units, List<IItem> loot)
        {
            List<GameUnit> unitsFromMap = new List<GameUnit>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (UnitEntity unit in units)
            {
                if (unit == null) continue;
                GameUnit unitToBuild = BuildUnitFromProperties(unit.Name, unit.Team, unit.Role, unit.IsCommander, unit,
                    loot);
                unitsFromMap.Add(unitToBuild);
            }

            return unitsFromMap;
        }

        public static GameUnit BuildUnitFromProperties(string id, Team unitTeam, Role unitJobClass, bool isCommander,
            UnitEntity mapEntity, List<IItem> loot)
        {
            GameUnit generatedUnit;

            if (unitTeam == Team.Creep)
            {
                generatedUnit = GenerateCreep(unitJobClass, unitTeam, id, isCommander, mapEntity as CreepEntity);
                PopulateUnitInventory(mapEntity.InitialInventory, loot, generatedUnit);
                AssignStartingGold(generatedUnit, ((CreepEntity) mapEntity).StartingGold, StartingGoldVariance);
            }
            else
            {
                generatedUnit = GenerateUnit(unitJobClass, unitTeam, id, isCommander, mapEntity);
            }

            return generatedUnit;
        }

        private static void AssignStartingGold(GameUnit generatedUnit, int amount, int variance)
        {
            generatedUnit.CurrentGold += amount + GameDriver.Random.Next(variance);
        }

        private static void PopulateUnitInventory(IEnumerable<string> inventoryItems, List<IItem> loot, GameUnit unit)
        {
            foreach (string itemName in inventoryItems)
            {
                IItem unitItem = loot.Find(item => item.Name == itemName);

                unit.AddItemToInventory(unitItem);
            }
        }

        #region Unit Statistics

        //UNITS


        private static UnitStatistics SelectChampionStats()
        {
            return new UnitStatistics(hp: 7, armor: 9, atk: 5, ret: 4, blk: 0, luck: 1, mv: 6, atkRange: new[] {1},
                maxCmd: 5);
        }

        private static UnitStatistics SelectMarauderStats()
        {
            return new UnitStatistics(hp: 18, armor: 0, atk: 5, ret: 5, blk: 0, luck: 0, mv: 6, atkRange: new[] {1},
                maxCmd: 5);
        }

        private static UnitStatistics SelectPaladinStats()
        {
            return new UnitStatistics(hp: 8, armor: 8, atk: 5, ret: 6, blk: 0, luck: 1, mv: 6, atkRange: new[] {1},
                maxCmd: 5);
        }

        private static UnitStatistics SelectBardStats()
        {
            return new UnitStatistics(hp: 8, armor: 4, atk: 3, ret: 3, blk: 0, luck: 1, mv: 5, atkRange: new[] {1, 2},
                maxCmd: 5);
        }

        private static UnitStatistics SelectClericStats()
        {
            return new UnitStatistics(hp: 6, armor: 6, atk: 0, ret: 0, blk: 0, luck: 4, mv: 6, atkRange: new[] {1, 2},
                maxCmd: 5);
        }

        private static UnitStatistics SelectCavalierStats()
        {
            return new UnitStatistics(hp: 8, armor: 6, atk: 6, ret: 4, blk: 0, luck: 1, mv: 6, atkRange: new[] {1},
                maxCmd: 5);
        }

        private static UnitStatistics SelectDuelistStats()
        {
            return new UnitStatistics(hp: 9, armor: 6, atk: 5, ret: 5, blk: 0, luck: 1, mv: 6, atkRange: new[] {1},
                maxCmd: 5);
        }

        private static UnitStatistics SelectPugilistStats()
        {
            return new UnitStatistics(hp: 9, armor: 6, atk: 6, ret: 4, blk: 0, luck: 0, mv: 6, atkRange: new[] {1},
                maxCmd: 5);
        }

        private static UnitStatistics SelectLancerStats()
        {
            return new UnitStatistics(hp: 9, armor: 6, atk: 6, ret: 4, blk: 0, luck: 1, mv: 6, atkRange: new[] {1},
                maxCmd: 5);
        }

        private static UnitStatistics SelectRogueStats()
        {
            return new UnitStatistics(hp: 8, armor: 7, atk: 5, ret: 4, blk: 0, luck: 2, mv: 6, atkRange: new[] {1},
                maxCmd: 5);
        }

        private static UnitStatistics SelectArcherStats()
        {
            return new UnitStatistics(hp: 7, armor: 6, atk: 6, ret: 4, blk: 0, luck: 1, mv: 5, atkRange: new[] {2},
                maxCmd: 5);
        }

        private static UnitStatistics SelectMageStats()
        {
            return new UnitStatistics(hp: 8, armor: 4, atk: 6, ret: 3, blk: 0, luck: 1, mv: 5, atkRange: new[] {1, 2},
                maxCmd: 5);
        }


        //PETS

        private static UnitStatistics SelectBoarStats()
        {
            return new UnitStatistics(hp: 5, armor: 3, atk: 3, ret: 3, blk: 0, luck: 0, mv: 5, atkRange: new[] {1},
                maxCmd: 5);
        }

        //CREEPS

        private static UnitStatistics SelectSlimeStats()
        {
            return new UnitStatistics(hp: 5, armor: 0, atk: 3, ret: 3, blk: 0, luck: 0, mv: 3, atkRange: new[] {1},
                maxCmd: 1);
        }

        private static UnitStatistics SelectTrollStats()
        {
            return new UnitStatistics(hp: 13, armor: 6, atk: 6, ret: 4, blk: 0, luck: 2, mv: 4, atkRange: new[] {1},
                maxCmd: 1);
        }

        private static UnitStatistics SelectOrcStats()
        {
            return new UnitStatistics(hp: 15, armor: 0, atk: 5, ret: 4, blk: 0, luck: 0, mv: 4, atkRange: new[] {1},
                maxCmd: 1);
        }

        private static UnitStatistics SelectNecromancerStats()
        {
            return new UnitStatistics(hp: 15, armor: 5, atk: 6, ret: 5, blk: 0, luck: 1, mv: 4, atkRange: new[] {1, 2},
                maxCmd: 1);
        }

        private static UnitStatistics SelectSkeletonStats()
        {
            return new UnitStatistics(hp: 5, armor: 2, atk: 4, ret: 4, blk: 0, luck: 0, mv: 4, atkRange: new[] {1},
                maxCmd: 1);
        }

        private static UnitStatistics SelectGoblinStats()
        {
            return new UnitStatistics(hp: 10, armor: 2, atk: 4, ret: 4, blk: 0, luck: 1, mv: 4, atkRange: new[] {1},
                maxCmd: 1);
        }

        private static UnitStatistics SelectRatStats()
        {
            return new UnitStatistics(hp: 10, armor: 0, atk: 3, ret: 3, blk: 0, luck: 0, mv: 5, atkRange: new[] {1},
                maxCmd: 1);
        }

        private static UnitStatistics SelectBatStats()
        {
            return new UnitStatistics(hp: 13, armor: 0, atk: 4, ret: 4, blk: 0, luck: 1, mv: 5, atkRange: new[] {1},
                maxCmd: 1);
        }

        private static UnitStatistics SelectSpiderStats()
        {
            return new UnitStatistics(hp: 6, armor: 6, atk: 4, ret: 4, blk: 0, luck: 0, mv: 5, atkRange: new[] {1},
                maxCmd: 1);
        }

        #endregion Unit Statistics

        #region Unit Skills

        private static List<UnitAction> SelectArcherSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new Draw(3, 1),
                new PoisonArrow(2, 4),
                new HuntingTrap(6, 1),
                new Sprint(2),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdHuntingCompanion(4));

            return skills;
        }

        private static List<UnitAction> SelectChampionSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new Bloodthirst(1),
                new Fortify(1, 1),
                new Challenge(2),
                new Tackle(),
                new Shove(),
                new Intervention(1, 1),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdWarmaster(5, 3, new[] {1, 2, 3}));

            return skills;
        }

        private static List<UnitAction> SelectMageSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new Inferno(3, 2),
                new Frostbite(1, 1),
                new Terraform(),
                new Replace(),
                new Sprint(2),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdTransfusion(4));

            return skills;
        }

        private static List<UnitAction> SelectLancerSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new LeapStrike(),
                new Execute(50, 3, 1),
                new Venom(2, 2),
                new Sprint(2),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdSonicStrike(4));

            return skills;
        }

        private static List<UnitAction> SelectBardSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new ModeSolo(),
                new ModeConcerto(),
                new SongAnthem(1, 2, new[] {0, 1, 2}),
                new SongTempest(1, 2, new[] {0, 1, 2}),
                new SongFreestyle(1, 2, new[] {0, 1, 2}),
                new VerseAccelerando(2, 1),
                new Sprint(2),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdSongBattleHymn(5, 1, 2, new[] {0, 1, 2}));

            return skills;
        }

        private static List<UnitAction> SelectPugilistSkills(bool isCommander)
        {
            const int flowStackDuration = 4;

            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new FlowStrike(75, flowStackDuration),
                new Uppercut(),
                new StemTheTide(2, flowStackDuration),
                new Meditate(),
                new Sprint(3),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdCoursingRiver(5, 2, flowStackDuration));

            return skills;
        }

        private static List<UnitAction> SelectDuelistSkills(bool isCommander)
        {
            const int maxFocusPoints = 2;
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new FadeStrike(),
                new PhaseStrike(),
                new Bloodthirst(2),
                new Sprint(2),
                new Guard(3),
                new Focus(maxFocusPoints)
            };

            if (isCommander) skills.Insert(1, new CmdPerfectFocus(4, maxFocusPoints));

            return skills;
        }

        private static List<UnitAction> SelectClericSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new Recover(3),
                new Bulwark(2, 2),
                new Atrophy(2, 2),
                new Cleanse(),
                new Sprint(2),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdGodsbreath(5, 1, 3));

            return skills;
        }

        private static List<UnitAction> SelectMarauderSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new Guillotine(50),
                new Rage(2, 3),
                new Grapple(),
                new Brace(2),
                new Shove(),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdBerserk(3));

            return skills;
        }

        private static List<UnitAction> SelectPaladinSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new Stun(1),
                new Rampart(3, 2),
                new Rescue(2),
                new Shove(),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdAngelicAssault(4));

            return skills;
        }

        private static List<UnitAction> SelectCavalierSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new Charge(4),
                new Bloodthirst(2),
                new PhaseStrike(),
                new Inspire(2, 1),
                new Sprint(3),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdDoubleTime(5, 2, 2));

            return skills;
        }

        private static List<UnitAction> SelectRogueSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new ThrowingKnife(),
                new BetwixtPlate(75),
                new Rend(3, 3),
                new Rob(),
                new PickDoorLock(10),
                new Sprint(3),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdAssassinate(5, 5));

            return skills;
        }

        private static List<UnitAction> SelectBoarSkills(bool isCommander)
        {
            List<UnitAction> skills = new List<UnitAction>
            {
                new BasicAttack(),
                new Guard(3),
                new Wait()
            };

            if (isCommander) skills.Insert(1, new CmdPerfectFocus(4, 2));

            return skills;
        }

        #endregion Unit Skills


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

        public static CreepUnit GenerateAdHocCreep(Role role, Dictionary<string, string> entityProperties,
            bool? isCommander = null, string[] initialInventory = null)
        {
            string unitName = NameGenerator.GenerateUnitName(role);

            CreepEntity generatedEntity = GenerateCreepEntity(unitName, "Creep", role, Team.Creep, isCommander ?? false,
                initialInventory ?? new string[0],
                AssetManager.UnitSprites, Vector2.Zero, entityProperties);

            CreepUnit creep = GenerateCreep(role, Team.Creep, unitName, false, generatedEntity);
            AssignStartingGold(creep, generatedEntity.StartingGold, StartingGoldVariance);
            return creep;
        }

        public static GameUnit GenerateAdHocUnit(Role role, Team team, bool isCommander)
        {
            string unitName = NameGenerator.GenerateUnitName(role);

            if (team == Team.Creep)
            {
                throw new Exception("Cannot create creep as if it were a player unit!");
            }

            UnitEntity generatedEntity = GenerateUnitEntity(unitName, "Unit", role, team, isCommander, new string[0],
                AssetManager.UnitSprites, Vector2.Zero);

            return GenerateUnit(role, team, unitName, isCommander, generatedEntity);
        }

        private static CreepUnit GenerateCreep(Role role, Team team, string unitName, bool isCommander,
            CreepEntity entity)
        {
            ITexture2D portrait =
                FindSmallPortrait(team.ToString(), role.ToString(), AssetManager.SmallPortraitTextures);

            UnitStatistics unitStatistics = GetUnitStatistics(role);

            return new CreepUnit(unitName, team, role, entity, unitStatistics, portrait, isCommander);
        }

        private static GameUnit GenerateUnit(Role role, Team team, string unitName, bool isCommander, UnitEntity entity)
        {
            ITexture2D portrait =
                FindSmallPortrait(team.ToString(), role.ToString(), AssetManager.SmallPortraitTextures);

            UnitStatistics unitStatistics = GetUnitStatistics(role);
            List<UnitAction> unitActions = GetUnitActions(role, isCommander);

            return new GameUnit(unitName, team, role, entity, unitStatistics, portrait, unitActions, isCommander);
        }

        private static UnitStatistics GetUnitStatistics(Role unitType)
        {
            switch (unitType)
            {
                case Role.Archer:
                    return SelectArcherStats();
                case Role.Champion:
                    return SelectChampionStats();
                case Role.Mage:
                    return SelectMageStats();
                case Role.Lancer:
                    return SelectLancerStats();
                case Role.Bard:
                    return SelectBardStats();
                case Role.Pugilist:
                    return SelectPugilistStats();
                case Role.Duelist:
                    return SelectDuelistStats();
                case Role.Cleric:
                    return SelectClericStats();
                case Role.Marauder:
                    return SelectMarauderStats();
                case Role.Paladin:
                    return SelectPaladinStats();
                case Role.Cavalier:
                    return SelectCavalierStats();
                case Role.Rogue:
                    return SelectRogueStats();
                case Role.Boar:
                    return SelectBoarStats();
                case Role.Slime:
                    return SelectSlimeStats();
                case Role.Troll:
                    return SelectTrollStats();
                case Role.Orc:
                    return SelectOrcStats();
                case Role.Necromancer:
                    return SelectNecromancerStats();
                case Role.Skeleton:
                    return SelectSkeletonStats();
                case Role.Goblin:
                    return SelectGoblinStats();
                case Role.Rat:
                    return SelectRatStats();
                case Role.Bat:
                    return SelectBatStats();
                case Role.Spider:
                    return SelectSpiderStats();
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }

        public static List<UnitAction> GetUnitActions(Role unitType, bool isCommander)
        {
            switch (unitType)
            {
                case Role.Archer:
                    return SelectArcherSkills(isCommander);
                case Role.Champion:
                    return SelectChampionSkills(isCommander);
                case Role.Mage:
                    return SelectMageSkills(isCommander);
                case Role.Lancer:
                    return SelectLancerSkills(isCommander);
                case Role.Bard:
                    return SelectBardSkills(isCommander);
                case Role.Pugilist:
                    return SelectPugilistSkills(isCommander);
                case Role.Duelist:
                    return SelectDuelistSkills(isCommander);
                case Role.Cleric:
                    return SelectClericSkills(isCommander);
                case Role.Marauder:
                    return SelectMarauderSkills(isCommander);
                case Role.Paladin:
                    return SelectPaladinSkills(isCommander);
                case Role.Cavalier:
                    return SelectCavalierSkills(isCommander);
                case Role.Rogue:
                    return SelectRogueSkills(isCommander);
                case Role.Boar:
                    return SelectBoarSkills(isCommander);
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }

        public static UnitEntity GenerateMapEntity(string name, string type, Role role, Team team, bool isCommander,
            string[] initialInventory, List<ITexture2D> unitSprites, Vector2 mapCoordinates,
            Dictionary<string, string> unitProperties)
        {
            return team == Team.Creep
                ? GenerateCreepEntity(name, type, role, team, isCommander, initialInventory, unitSprites,
                    mapCoordinates, unitProperties)
                : GenerateUnitEntity(name, type, role, team, isCommander, initialInventory, unitSprites,
                    mapCoordinates);
        }

        private static CreepEntity GenerateCreepEntity(string name, string type, Role role, Team team, bool isCommander,
            string[] initialInventory, List<ITexture2D> unitSprites, Vector2 mapCoordinates,
            Dictionary<string, string> creepProperties)
        {
            UnitSpriteSheet animatedSpriteSheet = GenerateUnitSpriteSheet(role, team, unitSprites);
            CreepRoutineModel creepRoutineModel = CreepRoutineModel.GetModelForCreep(creepProperties);
            CreepEntity creepEntity = new CreepEntity(name, type, animatedSpriteSheet, mapCoordinates, team, role,
                isCommander, creepRoutineModel, initialInventory);

            return creepEntity;
        }

        private static UnitEntity GenerateUnitEntity(string name, string type, Role role, Team team, bool isCommander,
            string[] initialInventory, List<ITexture2D> unitSprites, Vector2 mapCoordinates)
        {
            UnitSpriteSheet animatedSpriteSheet = GenerateUnitSpriteSheet(role, team, unitSprites);

            UnitEntity unitEntity = new UnitEntity(name, type, animatedSpriteSheet, mapCoordinates, team, role,
                isCommander, initialInventory);

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