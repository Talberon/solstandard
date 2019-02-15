using Microsoft.Xna.Framework;

namespace SolStandard.Entity
{
    public interface IRemotelyTriggerable
    {
        void RemoteTrigger();
        Vector2 MapCoordinates { get; }
    }
}