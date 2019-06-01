using Microsoft.Xna.Framework;

namespace SolStandard.Entity
{
    public interface ITriggerable
    {
        string Name { get; }
        Vector2 MapCoordinates { get; }
        bool CanTrigger { get; }
        int[] InteractRange { get; }
        void Trigger();
    }
}