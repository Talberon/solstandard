using SolStandard.Map.Elements;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public enum UnitClass
    {
        Champion,
        Archer,
        Mage
    }

    public enum Team
    {
        Red,
        Blue
    }

    public class GameUnit : GameEntity
    {
        private readonly Team unitTeam;
        private readonly UnitClass unitJobClass;

        private readonly ITexture2D largePortrait;
        private readonly ITexture2D mediumPortrait;
        private readonly ITexture2D smallPortrait;

        private readonly UnitStatistics stats;

        public GameUnit(string id, Team unitTeam, UnitClass unitJobClass, MapEntity mapInfo, UnitStatistics stats,
            ITexture2D largePortrait, ITexture2D mediumPortrait, ITexture2D smallPortrait) : base(id, mapInfo)
        {
            this.unitTeam = unitTeam;
            this.unitJobClass = unitJobClass;
            this.stats = stats;
            this.largePortrait = largePortrait;
            this.mediumPortrait = mediumPortrait;
            this.smallPortrait = smallPortrait;
        }

        public UnitStatistics Stats
        {
            get { return stats; }
        }

        public Team UnitTeam
        {
            get { return unitTeam; }
        }

        public UnitClass UnitJobClass
        {
            get { return unitJobClass; }
        }

        public ITexture2D LargePortrait
        {
            get { return largePortrait; }
        }

        public ITexture2D MediumPortrait
        {
            get { return mediumPortrait; }
        }

        public ITexture2D SmallPortrait
        {
            get { return smallPortrait; }
        }
    }
}