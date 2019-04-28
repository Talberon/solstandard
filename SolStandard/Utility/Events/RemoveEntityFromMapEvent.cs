using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Map;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Events
{
    public class RemoveEntityFromMapEvent : IEvent
    {
        private readonly Vector2 coordinatesToClear;
        private readonly Layer mapLayer;
        private readonly ISoundEffect soundEffect;
        public bool Complete { get; private set; }

        public RemoveEntityFromMapEvent(Layer mapLayer, Vector2 coordinatesToClear, ISoundEffect soundEffect = null)
        {
            this.mapLayer = mapLayer;
            this.coordinatesToClear = coordinatesToClear;
            this.soundEffect = soundEffect;
        }

        public void Continue()
        {
            MapContainer.GameGrid[(int) mapLayer][(int) coordinatesToClear.X, (int) coordinatesToClear.Y] = null;

            if (soundEffect != null) soundEffect.Play();

            Complete = true;
        }
    }
}