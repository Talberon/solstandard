
using Microsoft.Xna.Framework;

namespace SolStandard.Entity
{
    public enum EffectTriggerTime
    {
        StartOfTurn,
        EndOfTurn
    }
    
    public interface IEffectTile
    {
        void Trigger(EffectTriggerTime triggerTime);
        bool IsExpired { get; }
        Vector2 MapCoordinates { get; }
    }
}