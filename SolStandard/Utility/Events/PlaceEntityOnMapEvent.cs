using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Events
{
    public class PlaceEntityOnMapEvent : IEvent
    {
        private readonly MapEntity entityToPlace;
        private readonly Layer mapLayer;
        private readonly ISoundEffect soundEffect;

        public PlaceEntityOnMapEvent(MapEntity entityToPlace, Layer mapLayer, ISoundEffect soundEffect)
        {
            this.entityToPlace = entityToPlace;
            this.mapLayer = mapLayer;
            this.soundEffect = soundEffect;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            MapContainer.GameGrid[(int) mapLayer][(int) entityToPlace.MapCoordinates.X,
                (int) entityToPlace.MapCoordinates.Y] = entityToPlace;

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                "Placed " + entityToPlace.Name + "!",
                entityToPlace.MapCoordinates,
                50
            );
            
            soundEffect.Play();

            GameMapContext.GameMapView.GenerateObjectiveWindow();
            
            Complete = true;
        }
    }
}