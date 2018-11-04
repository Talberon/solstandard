using System;
using System.Collections.Generic;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Archer;
using SolStandard.Entity.Unit.Actions.Champion;
using SolStandard.Entity.Unit.Actions.Mage;
using SolStandard.Entity.Unit.Actions.Monarch;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public static class UnitGenerator
    {
        private static List<ITexture2D> _largePortraits;
        private static List<ITexture2D> _mediumPortraits;
        private static List<ITexture2D> _smallPortraits;

        public static List<GameUnit> GenerateUnitsFromMap(IEnumerable<UnitEntity> units,
            List<ITexture2D> largePortraits, List<ITexture2D> mediumPortraits, List<ITexture2D> smallPortraits)
        {
            _largePortraits = largePortraits;
            _mediumPortraits = mediumPortraits;
            _smallPortraits = smallPortraits;

            List<GameUnit> unitsFromMap = new List<GameUnit>();

            foreach (UnitEntity unit in units)
            {
                if (unit == null) continue;


                Team unitTeam;

                switch (unit.TiledProperties["Team"])
                {
                    case "Red":
                        unitTeam = Team.Red;
                        break;
                    case "Blue":
                        unitTeam = Team.Blue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("", unit.TiledProperties["Team"], null);
                }

                Role role;

                switch (unit.TiledProperties["Class"])
                {
                    case "Archer":
                        role = Role.Archer;
                        break;
                    case "Champion":
                        role = Role.Champion;
                        break;
                    case "Mage":
                        role = Role.Mage;
                        break;
                    case "Monarch":
                        role = Role.Monarch;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("", unit.TiledProperties["Class"], null);
                }

                GameUnit unitToBuild = BuildUnitFromProperties(unit.Name, unitTeam, role, unit);
                unitsFromMap.Add(unitToBuild);
            }

            return unitsFromMap;
        }

        private static GameUnit BuildUnitFromProperties(string id, Team unitTeam, Role unitJobClass,
            UnitEntity mapEntity)
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
                case Role.Monarch:
                    unitStats = SelectMonarchStats();
                    unitSkills = SelectMonarchSkills();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unitJobClass", unitJobClass, null);
            }

            return new GameUnit(id, unitTeam, unitJobClass, mapEntity, unitStats, largePortrait, mediumPortrait,
                smallPortrait, unitSkills);
        }

        private static UnitStatistics SelectArcherStats()
        {
            return new UnitStatistics(7, 4, 4, 3, 4, new[] {2});
        }

        private static UnitStatistics SelectChampionStats()
        {
            return new UnitStatistics(5, 9, 4, 1, 5, new[] {1});
        }

        private static UnitStatistics SelectMageStats()
        {
            return new UnitStatistics(8, 2, 5, 2, 4, new[] {1, 2});
        }

        private static UnitStatistics SelectMonarchStats()
        {
            return new UnitStatistics(20, 0, 4, 3, 4, new[] {1});
        }

        private static List<UnitAction> SelectArcherSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Harpoon(2),
                new Draw(2, 1),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> SelectChampionSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Cover(3),
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
                new Ignite(3, 2),
                new Atrophy(2, 2),
                new Blink(),
                new Guard(3),
                new Wait()
            };
        }

        private static List<UnitAction> SelectMonarchSkills()
        {
            return new List<UnitAction>
            {
                new BasicAttack(),
                new Inspire(2, 1),
                new DoubleTime(1, 2),
                new Bulwark(2, 2),
                new Wait()
            };
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