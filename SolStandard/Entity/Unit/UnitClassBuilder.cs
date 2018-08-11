using System;
using System.Collections.Generic;
using SolStandard.Map;
using SolStandard.Map.Objects;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public class UnitClassBuilder
    {
        private readonly List<ITexture2D> largePortraits;
        private readonly List<ITexture2D> mediumPortraits;
        private readonly List<ITexture2D> smallPortraits;

        public UnitClassBuilder(List<ITexture2D> largePortraits, List<ITexture2D> mediumPortraits,
            List<ITexture2D> smallPortraits)
        {
            this.largePortraits = largePortraits;
            this.mediumPortraits = mediumPortraits;
            this.smallPortraits = smallPortraits;
        }

        public static List<GameUnit> GenerateUnitsFromMap(MapEntity[,] unitLayer,
            List<ITexture2D> largePortraitTextures,
            List<ITexture2D> mediumPortraitTextures, List<ITexture2D> smallPortraitTextures)
        {
            List<GameUnit> unitsFromMap = new List<GameUnit>();

            foreach (MapEntity unit in unitLayer)
            {
                if (unit == null) continue;
                
                UnitClassBuilder unitBuilder = new UnitClassBuilder(largePortraitTextures, mediumPortraitTextures,
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
                        throw new ArgumentOutOfRangeException("unitTeam", unit.TiledProperties["Team"], null);
                }

                UnitClass unitClass;

                switch (unit.TiledProperties["Class"])
                {
                    case "Archer":
                        unitClass = UnitClass.Archer;
                        break;
                    case "Champion":
                        unitClass = UnitClass.Champion;
                        break;
                    case "Mage":
                        unitClass = UnitClass.Mage;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("unitClass", unit.TiledProperties["Class"], null);
                }

                GameUnit unitToBuild = unitBuilder.BuildUnitFromProperties(unit.Name, unitTeam, unitClass, unit);
                unitsFromMap.Add(unitToBuild);
            }

            return unitsFromMap;
        }

        private GameUnit BuildUnitFromProperties(string id, Team unitTeam, UnitClass unitJobClass,
            MapEntity mapEntity)
        {
            string unitTeamAndClass = unitTeam.ToString() + "/" + unitJobClass.ToString();

            ITexture2D smallPortrait = GetSmallPortrait(unitTeamAndClass);
            ITexture2D mediumPortrait = GetMediumPortrait(unitTeamAndClass);
            ITexture2D largePortrait = GetLargePortrait(unitTeamAndClass);

            UnitStatistics unitStats;

            switch (unitJobClass)
            {
                case UnitClass.Archer:
                    unitStats = SelectArcherStats();
                    break;
                case UnitClass.Champion:
                    unitStats = SelectChampionStats();
                    break;
                case UnitClass.Mage:
                    unitStats = SelectMageStats();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unitJobClass", unitJobClass, null);
            }

            return new GameUnit(id, unitTeam, unitJobClass, mapEntity, unitStats, largePortrait, mediumPortrait,
                smallPortrait);
        }

        private static UnitStatistics SelectArcherStats()
        {
            return new UnitStatistics(100, 20, 5, 3, 1, 7, new[] {2});
        }

        private static UnitStatistics SelectChampionStats()
        {
            return new UnitStatistics(100, 20, 10, 5, 1, 6, new[] {1});
        }

        private static UnitStatistics SelectMageStats()
        {
            return new UnitStatistics(100, 30, 0, 5, 1, 5, new[] {1, 2});
        }

        private ITexture2D GetLargePortrait(string textureName)
        {
            return largePortraits.Find(texture => texture.GetTexture2D().Name.Contains(textureName));
        }

        private ITexture2D GetMediumPortrait(string textureName)
        {
            return mediumPortraits.Find(texture => texture.GetTexture2D().Name.Contains(textureName));
        }

        private ITexture2D GetSmallPortrait(string textureName)
        {
            return smallPortraits.Find(texture => texture.GetTexture2D().Name.Contains(textureName));
        }
    }
}