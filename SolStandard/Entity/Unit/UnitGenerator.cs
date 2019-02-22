﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Archer;
using SolStandard.Entity.Unit.Actions.Bard;
using SolStandard.Entity.Unit.Actions.Champion;
using SolStandard.Entity.Unit.Actions.Creeps;
using SolStandard.Entity.Unit.Actions.Lancer;
using SolStandard.Entity.Unit.Actions.Mage;
using SolStandard.Entity.Unit.Actions.Pugilist;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public static class UnitGenerator
    {
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
            {"Bard", Role.Bard},
            {"Pugilist", Role.Pugilist},
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


        public static List<GameUnit> GenerateUnitsFromMap(IEnumerable<UnitEntity> units, List<IItem> loot)
        {
            List<GameUnit> unitsFromMap = new List<GameUnit>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (UnitEntity unit in units)
            {
                if (unit == null) continue;

                Team unitTeam = TeamDictionary[unit.TiledProperties["Team"]];
                Role role = RoleDictionary[unit.TiledProperties["Class"]];
                bool commander = Convert.ToBoolean(unit.TiledProperties["Commander"]);

                GameUnit unitToBuild = BuildUnitFromProperties(unit.Name, unitTeam, role, commander, unit, loot);
                unitsFromMap.Add(unitToBuild);
            }

            return unitsFromMap;
        }

        private static GameUnit BuildUnitFromProperties(string id, Team unitTeam, Role unitJobClass, bool isCommander,
            UnitEntity mapEntity, List<IItem> loot)
        {
            GameUnit generatedUnit = GenerateUnit(unitJobClass, unitTeam, id, isCommander, mapEntity,
                mapEntity.TiledProperties);

            if (generatedUnit.Team == Team.Creep)
            {
                PopulateUnitInventoryAndTradeActions(mapEntity, loot, generatedUnit);
                AssignStartingGold(generatedUnit);
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
                case Role.Merchant:
                    generatedUnit.CurrentGold += 5 + GameDriver.Random.Next(10);
                    break;
            }
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

        private static bool UnitHasItemsToTrade(MapEntity mapEntity, IReadOnlyCollection<int> itemPrices)
        {
            return Convert.ToBoolean(mapEntity.TiledProperties["WillTrade"]) && itemPrices.Count > 0;
        }

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

        private static UnitStatistics SelectMerchantStats()
        {
            return new UnitStatistics(hp: 20, armor: 15, atk: 5, ret: 8, luck: 3, mv: 6, atkRange: new[] {1, 2});
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
                new Ignite(3, 3),
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
                new LeapStrike(3),
                new PoisonTip(2, 4),
                new Execute(3),
                new Guard(3),
                new DropGiveGoldAction(),
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
                new Guard(3),
                new DropGiveGoldAction(),
                new Wait()
            };
        }

        private static List<UnitAction> SelectPugilistSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Guard(3),
                new Uppercut(),
                new PressurePoint(),
                new Meditate(),
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

        public static GameUnit GenerateDraftUnit(Role role, Team team, bool isCommander)
        {
            string unitName = NameGenerator.GenerateUnitName(role);
            const string type = "Unit";
            Dictionary<string, string> tiledProperties = new Dictionary<string, string>();
            Vector2 mapCoordinates = Vector2.Zero;

            UnitEntity generatedEntity = GenerateUnitEntity(unitName, type, role, team, isCommander,
                AssetManager.UnitSprites, mapCoordinates, tiledProperties);

            return GenerateUnit(role, team, unitName, isCommander, generatedEntity, tiledProperties);
        }

        private static GameUnit GenerateUnit(Role role, Team team, string unitName, bool isCommander, UnitEntity entity,
            IReadOnlyDictionary<string, string> tiledProperties)
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
                case Role.Slime:
                    unitStatistics = SelectSlimeStats();
                    unitActions = SelectCreepRoutine(tiledProperties);
                    break;
                case Role.Troll:
                    unitStatistics = SelectTrollStats();
                    unitActions = SelectCreepRoutine(tiledProperties);
                    break;
                case Role.Orc:
                    unitStatistics = SelectOrcStats();
                    unitActions = SelectCreepRoutine(tiledProperties);
                    break;
                case Role.Merchant:
                    unitStatistics = SelectMerchantStats();
                    unitActions = SelectCreepRoutine(tiledProperties);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("role", role, null);
            }

            GameUnit unit =
                new GameUnit(unitName, team, role, entity, unitStatistics, portrait, unitActions, isCommander);

            return unit;
        }


        public static UnitEntity GenerateUnitEntity(string name, string type, Role role, Team team, bool isCommander,
            List<ITexture2D> unitSprites, Vector2 mapCoordinates, Dictionary<string, string> unitProperties)
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

            UnitEntity unitEntity = new UnitEntity(name, type,
                animatedSpriteSheet, mapCoordinates, isCommander, unitProperties);

            return unitEntity;
        }

        private static ITexture2D FetchUnitGraphic(string unitTeam, string role, List<ITexture2D> unitSprites)
        {
            string unitTeamAndClass = unitTeam + role;
            return unitSprites.Find(texture => texture.Name.Contains(unitTeamAndClass));
        }
    }
}