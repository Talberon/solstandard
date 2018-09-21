using System;
using System.Collections.Generic;
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

            ITexture2D smallPortrait = FindSmallPortrait(unitTeamAndClass);
            ITexture2D mediumPortrait = FindMediumPortrait(unitTeamAndClass);
            ITexture2D largePortrait = FindLargePortrait(unitTeamAndClass);

            UnitStatistics unitStats;

            switch (unitJobClass)
            {
                case Role.Archer:
                    unitStats = SelectArcherStats();
                    break;
                case Role.Champion:
                    unitStats = SelectChampionStats();
                    break;
                case Role.Mage:
                    unitStats = SelectMageStats();
                    break;
                case Role.Monarch:
                    unitStats = SelectMonarchStats();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unitJobClass", unitJobClass, null);
            }

            return new GameUnit(id, unitTeam, unitJobClass, mapEntity, unitStats, largePortrait, mediumPortrait,
                smallPortrait);
        }

        private static UnitStatistics SelectArcherStats()
        {
            return new UnitStatistics(5, 4, 2, 1, 4, new[] {2});
        }

        private static UnitStatistics SelectChampionStats()
        {
            return new UnitStatistics(7, 4, 3, 1, 5, new[] {1});
        }

        private static UnitStatistics SelectMageStats()
        {
            return new UnitStatistics(3, 5, 1, 1, 4, new[] {1, 2});
        }

        private static UnitStatistics SelectMonarchStats()
        {
            return new UnitStatistics(10, 2, 2, 1, 3, new[] {1});
        }

        private ITexture2D FindLargePortrait(string textureName)
        {
            return largePortraits.Find(texture => texture.MonoGameTexture.Name.Contains(textureName));
        }

        private ITexture2D FindMediumPortrait(string textureName)
        {
            return mediumPortraits.Find(texture => texture.MonoGameTexture.Name.Contains(textureName));
        }

        private ITexture2D FindSmallPortrait(string textureName)
        {
            return smallPortraits.Find(texture => texture.MonoGameTexture.Name.Contains(textureName));
        }
    }
}