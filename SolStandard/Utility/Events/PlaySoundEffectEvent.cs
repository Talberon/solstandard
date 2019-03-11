using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Events
{
    public class PlaySoundEffectEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly ISoundEffect soundEffect;

        public PlaySoundEffectEvent(ISoundEffect soundEffect)
        {
            this.soundEffect = soundEffect;
        }

        public void Continue()
        {
            soundEffect.Play();
            Complete = true;
        }
    }
}