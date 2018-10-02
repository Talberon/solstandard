using System;
using System.Collections.Generic;
using SolStandard.Entity.Unit.Skills;
using SolStandard.Entity.Unit.Skills.Archer;
using SolStandard.Entity.Unit.Skills.Champion;
using SolStandard.Entity.Unit.Skills.Mage;
using SolStandard.Entity.Unit.Skills.Monarch;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public class UnitBuilder
    {
        private readonly List<ITexture2D> largePortraits;
        private readonly List<ITexture2D> mediumPortraits;
        private readonly List<ITexture2D> smallPortraits;

        public UnitBuilder(List<ITexture2D> largePortraits, List<ITexture2D> mediumPortraits,
            List<ITexture2D> smallPortraits)
        {
            this.largePortraits = largePortraits;
            this.mediumPortraits = mediumPortraits;
            this.smallPortraits = smallPortraits;
        }

        public static List<GameUnit> GenerateUnitsFromMap(IEnumerable<UnitEntity> units,
            List<ITexture2D> largePortraitTextures,
            List<ITexture2D> mediumPortraitTextures, List<ITexture2D> smallPortraitTextures)
        {
            List<GameUnit> unitsFromMap = new List<GameUnit>();

            foreach (UnitEntity unit in units)
            {
                if (unit == null) continue;

                UnitBuilder unitBuilder = new UnitBuilder(largePortraitTextures, mediumPortraitTextures,
                    smallPortraitTextures);

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

                GameUnit unitToBuild = unitBuilder.BuildUnitFromProperties(unit.Name, unitTeam, role, unit);
                unitsFromMap.Add(unitToBuild);
            }

            return unitsFromMap;
        }

        private GameUnit BuildUnitFromProperties(string id, Team unitTeam, Role unitJobClass, UnitEntity mapEntity)
        {
            string unitTeamAndClass = unitTeam.ToString() + "/" + unitJobClass.ToString();

            ITexture2D smallPortrait = FindSmallPortrait(unitTeam.ToString(), unitJobClass.ToString());
            ITexture2D mediumPortrait = FindMediumPortrait(unitTeam.ToString(), unitJobClass.ToString());
            ITexture2D largePortrait = FindLargePortrait(unitTeam.ToString(), unitJobClass.ToString());

            UnitStatistics unitStats;
            List<UnitSkill> unitSkills;

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
            return new UnitStatistics(4, 4, 2, 1, 4, new[] {2});
        }

        private static UnitStatistics SelectChampionStats()
        {
            return new UnitStatistics(4, 4, 3, 1, 5, new[] {1});
        }

        private static UnitStatistics SelectMageStats()
        {
            return new UnitStatistics(3, 5, 1, 1, 4, new[] {1, 2});
        }

        private static UnitStatistics SelectMonarchStats()
        {
            return new UnitStatistics(6, 3, 1, 1, 3, new[] {1, 2});
        }

        private static List<UnitSkill> SelectArcherSkills()
        {
            return new List<UnitSkill>
            {
                new BasicAttack(),
                new Draw(2, 1),
                new Shove(),
                new Wait()
            };
        }

        private static List<UnitSkill> SelectChampionSkills()
        {
            return new List<UnitSkill>
            {
                new BasicAttack(),
                new Cover(1, 2),
                new Tackle(),
                new Shove(),
                new Wait()
            };
        }

        private static List<UnitSkill> SelectMageSkills()
        {
            return new List<UnitSkill>
            {
                new BasicAttack(),
                new Blink(),
                new Shove(),
                new Wait()
            };
        }

        private static List<UnitSkill> SelectMonarchSkills()
        {
            return new List<UnitSkill>
            {
                new BasicAttack(),
                new Inspire(2, 1),
                new DoubleTime(1, 2),
                new Shove(),
                new Wait()
            };
        }


        private ITexture2D FindLargePortrait(string unitTeam, string unitJobClass)
        {
            return largePortraits.Find(texture =>
                texture.MonoGameTexture.Name.Contains(unitTeam) && texture.MonoGameTexture.Name.Contains(unitJobClass));
        }

        private ITexture2D FindMediumPortrait(string unitTeam, string unitJobClass)
        {
            return mediumPortraits.Find(texture =>
                texture.MonoGameTexture.Name.Contains(unitTeam) && texture.MonoGameTexture.Name.Contains(unitJobClass));
        }

        private ITexture2D FindSmallPortrait(string unitTeam, string unitJobClass)
        {
            return smallPortraits.Find(texture =>
                texture.MonoGameTexture.Name.Contains(unitTeam) && texture.MonoGameTexture.Name.Contains(unitJobClass));
        }
    }
}