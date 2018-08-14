using System.Collections.Generic;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers
{
    public class GameContainer
    {
        private readonly List<GameUnit> units;
        private readonly MapLayer mapLayer;
        private readonly MapScene mapScene;

        public GameContainer(MapLayer mapLayer, MapScene mapScene, List<GameUnit> units)
        {
            this.mapLayer = mapLayer;
            this.mapScene = mapScene;
            this.units = units;
        }

        public MapLayer GetMapLayer()
        {
            return mapLayer;
        }

        public MapScene GetMapScene()
        {
            return mapScene;
        }

        public List<GameUnit> GetUnits()
        {
            return units;
        }
    }
}