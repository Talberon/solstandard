using Microsoft.Xna.Framework;

namespace SolStandard.Entity
{
    public enum EffectTriggerTime
    {
        StartOfRound,
        EndOfTurn
    }

    public interface IEffectTile
    {
        bool Trigger(EffectTriggerTime triggerTime);
        bool WillTrigger(EffectTriggerTime triggerTime);
        bool HasTriggered { get; set; }
        bool IsExpired { get; }
        Vector2 MapCoordinates { get; }
    }
}