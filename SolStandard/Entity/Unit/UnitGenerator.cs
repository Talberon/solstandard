using System;
using System.Collections.Generic;
using System.Linq;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Archer;
using SolStandard.Entity.Unit.Actions.Champion;
using SolStandard.Entity.Unit.Actions.Creeps;
using SolStandard.Entity.Unit.Actions.Mage;
using SolStandard.Entity.Unit.Actions.Monarch;
using SolStandard.Map.Elements;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public static class UnitGenerator
    {
        private static List<ITexture2D> _largePortraits;
        private static List<ITexture2D> _mediumPortraits;
        private static List<ITexture2D> _smallPortraits;

        private enum Routine
        {
            Roam,
            Wander
        }

        private static readonly Dictionary<string, Team> TeamDictionary = new Dictionary<string, Team>
        {
            {"Red", Team.Red},
            {"Blue", Team.Blue},
            {"Creep", Team.Creep}
        };

        private static readonly Dictionary<string, Role> RoleDictionary = new Dictionary<string, Role>
        {
            {"Archer", Role.Archer},
            {"Champion", Role.Champion},
            {"Mage", Role.Mage},
            {"Lancer", Role.Lancer},
            {"Monarch", Role.Monarch},
            {"Slime", Role.Slime},
            {"Troll", Role.Troll},
            {"Orc", Role.Orc},
            {"Merchant", Role.Merchant}
        };

        private static readonly Dictionary<string, Routine> RoutineDictionary = new Dictionary<string, Routine>
        {
            {"Roam", Routine.Roam},
            {"Wander", Routine.Wander}
        };

        public static List<GameUnit> GenerateUnitsFromMap(IEnumerable<UnitEntity> units, List<IItem> loot,
            List<ITexture2D> largePortraits, List<ITexture2D> mediumPortraits, List<ITexture2D> smallPortraits)
        {
            _largePortraits = largePortraits;
            _mediumPortraits = mediumPortraits;
            _smallPortraits = smallPortraits;

            List<GameUnit> unitsFromMap = new List<GameUnit>();

            foreach (UnitEntity unit in units)
            {
                if (unit == null) continue;

                Team unitTeam = TeamDictionary[unit.TiledProperties["Team"]];
                Role role = RoleDictionary[unit.TiledProperties["Class"]];

                GameUnit unitToBuild = BuildUnitFromProperties(unit.Name, unitTeam, role, unit, loot);
                unitsFromMap.Add(unitToBuild);
            }

            return unitsFromMap;
        }

        private static GameUnit BuildUnitFromProperties(string id, Team unitTeam, Role unitJobClass,
            UnitEntity mapEntity, List<IItem> loot)
        {
            ITexture2D smallPortrait = FindSmallPortrait(unitTeam.ToString(), unitJobClass.ToString());
            ITexture2D mediumPortrait = FindMediumPortrait(unitTeam.ToString(), unitJobClass.ToString());
            ITexture2D largePortrait = FindLargePortrait(unitTeam.ToString(), unitJobClass.ToString());

            UnitStatistics unitStats;
            List<UnitAction> unitSkills;

            switch (unitJobClass)
            {
                case Role.Archer:
                    unitStats = SelectArcherStats();
                    unitSkills = SelectArcherSkills();
                    break;
                case Role.Champion:
                    unitStats = SelectChampionStats();
                    unitSkills = SelectChampionSkills();
                    break;
                case Role.Mage:
                    unitStats = SelectMageStats();
                    unitSkills = SelectMageSkills();
                    break;
                case Role.Lancer:
                    unitStats = SelectLancerStats();
                    unitSkills = SelectLancerSkills();
                    break;
                case Role.Monarch:
                    unitStats = SelectMonarchStats();
                    unitSkills = SelectMonarchSkills();
                    break;
                case Role.Slime:
                    unitStats = SelectSlimeStats();
                    unitSkills = SelectCreepRoutine(mapEntity.TiledProperties);
                    break;
                case Role.Troll:
                    unitStats = SelectTrollStats();
                    unitSkills = SelectCreepRoutine(mapEntity.TiledProperties);
                    break;
                case Role.Orc:
                    unitStats = SelectOrcStats();
                    unitSkills = SelectCreepRoutine(mapEntity.TiledProperties);
                    break;
                case Role.Merchant:
                    unitStats = SelectMerchantStats();
                    unitSkills = SelectCreepRoutine(mapEntity.TiledProperties);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unitJobClass", unitJobClass, null);
            }

            GameUnit generatedUnit = new GameUnit(id, unitTeam, unitJobClass, mapEntity, unitStats, largePortrait,
                mediumPortrait, smallPortrait, unitSkills);

            if (generatedUnit.Team == Team.Creep)
            {
                PopulateUnitInventoryAndTradeActions(mapEntity, loot, generatedUnit);
            }

            switch (generatedUnit.Role)
            {
                case Role.Champion:
                    break;
                case Role.Archer:
                    break;
                case Role.Mage:
                    break;
                case Role.Lancer:
                    break;
                case Role.Monarch:
                    break;
                case Role.Slime:
                    generatedUnit.CurrentGold += 3 + GameDriver.Random.Next(5);
                    break;
                case Role.Troll:
                    generatedUnit.CurrentGold += 14 + GameDriver.Random.Next(5);
                    break;
                case Role.Orc:
                    generatedUnit.CurrentGold += 7 + GameDriver.Random.Next(8);
                    break;
                case Role.Merchant:
                    generatedUnit.CurrentGold += 5 + GameDriver.Random.Next(10);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return generatedUnit;
        }

        private static void PopulateUnitInventoryAndTradeActions(MapEntity mapEntity, List<IItem> loot,
            GameUnit generatedUnit)
        {
            string itemNameProp = mapEntity.TiledProperties["Item"];
            if (itemNameProp == string.Empty) return;
            string[] unitInventory = itemNameProp.Split('|');


            string itemPricesProp = mapEntity.TiledProperties["ItemPrice"];
            int[] itemPrices = (itemPricesProp != string.Empty)
                ? itemPricesProp.Split('|').Select(int.Parse).ToArray()
                : new int[0];

            for (int i = 0; i < unitInventory.Length; i++)
            {
                IItem unitItem = loot.Find(item => item.Name == unitInventory[i]);

                generatedUnit.AddItemToInventory(unitItem);

                if (UnitHasItemsToTrade(mapEntity, itemPrices))
                {
                    int itemPrice = itemPrices[i];

                    if (itemPrice > 0)
                    {
                        generatedUnit.AddContextualAction(new BuyItemAction(unitItem, itemPrices[i]));
                    }
                }
            }
        }

        private static bool UnitHasItemsToTrade(MapEntity mapEntity, int[] itemPrices)
        {
            return Convert.ToBoolean(mapEntity.TiledProperties["WillTrade"]) && itemPrices.Length > 0;
        }

        private static UnitStatistics SelectArcherStats()
        {
            return new UnitStatistics(6, 4, 4, 3, 2, 5, new[] {2});
        }

        private static UnitStatistics SelectChampionStats()
        {
            return new UnitStatistics(7, 9, 5, 5, 1, 6, new[] {1});
        }

        private static UnitStatistics SelectMageStats()
        {
            return new UnitStatistics(5, 3, 6, 3, 2, 5, new[] {1, 2});
        }
        
        private static UnitStatistics SelectLancerStats()
        {
            return new UnitStatistics(9, 5, 6, 4, 2, 6, new[] {1});
        }

        private static UnitStatistics SelectMonarchStats()
        {
            return new UnitStatistics(20, 0, 4, 3, 1, 5, new[] {1});
        }

        private static UnitStatistics SelectSlimeStats()
        {
            return new UnitStatistics(7, 0, 3, 3, 0, 3, new[] {1});
        }

        private static UnitStatistics SelectTrollStats()
        {
            return new UnitStatistics(20, 3, 6, 4, 2, 4, new[] {1});
        }

        private static UnitStatistics SelectOrcStats()
        {
            return new UnitStatistics(15, 0, 5, 4, 0, 4, new[] {1});
        }

        private static UnitStatistics SelectMerchantStats()
        {
            return new UnitStatistics(20, 15, 5, 8, 3, 6, new[] {1, 2});
        }

        private static List<UnitAction> SelectArcherSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Draw(2, 1),
                new HuntingTrap(5, 1),
                new Harpoon(2),
                new Guard(3),
                new DropGiveGoldAction(),
                new Wait()
            };
        }

        private static List<UnitAction> SelectChampionSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Bloodthirst(2),
                new Tackle(),
                new Shove(),
                new Atrophy(2, 2),
                new Guard(3),
                new DropGiveGoldAction(),
                new Wait()
            };
        }

        private static List<UnitAction> SelectMageSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Ignite(2, 3),
                new Inferno(2, 3),
                new Replace(),
                new Guard(3),
                new DropGiveGoldAction(),
                new Wait()
            };
        }
        
        private static List<UnitAction> SelectLancerSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Guard(3),
                new DropGiveGoldAction(),
                new Wait()
            };
        }

        private static List<UnitAction> SelectMonarchSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new DoubleTime(2, 1),
                new Inspire(2, 1),
                new Bulwark(2, 2),
                new DropGiveGoldAction(),
                new Wait()
            };
        }

        private static List<UnitAction> SelectCreepRoutine(IReadOnlyDictionary<string, string> tiledProperties)
        {
            List<UnitAction> actions = new List<UnitAction>();

            switch (RoutineDictionary[tiledProperties["Routine"]])
            {
                case Routine.Roam:
                    actions.Add(new RoamingRoutine(Convert.ToBoolean(tiledProperties["Independent"])));
                    break;
                case Routine.Wander:
                    actions.Add(new RoamingRoutine(Convert.ToBoolean(tiledProperties["Independent"]), false));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return actions;
        }


        private static ITexture2D FindLargePortrait(string unitTeam, string unitJobClass)
        {
            return _largePortraits.Find(texture =>
                texture.MonoGameTexture.Name.Contains(unitTeam) && texture.MonoGameTexture.Name.Contains(unitJobClass));
        }

        private static ITexture2D FindMediumPortrait(string unitTeam, string unitJobClass)
        {
            return _mediumPortraits.Find(texture =>
                texture.MonoGameTexture.Name.Contains(unitTeam) && texture.MonoGameTexture.Name.Contains(unitJobClass));
        }

        private static ITexture2D FindSmallPortrait(string unitTeam, string unitJobClass)
        {
            return _smallPortraits.Find(texture =>
                texture.MonoGameTexture.Name.Contains(unitTeam) && texture.MonoGameTexture.Name.Contains(unitJobClass));
        }
    }
}