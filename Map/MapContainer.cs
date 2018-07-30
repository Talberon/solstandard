using SolStandard.Map.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolStandard.Map
{
    public class MapContainer
    {
        private List<MapObject[,]> gameGrid;

        public MapContainer(List<MapObject[,]> gameGrid)
        {
            this.gameGrid = gameGrid;
        }

        public ReadOnlyCollection<MapObject[,]> GetGameGrid()
        {
            return gameGrid.AsReadOnly();
        }
    }
}
