using Microsoft.Xna.Framework.Audio;

namespace SolStandard.Utility.Monogame
{
    public class SoundEffectWrapper : ISoundEffect
    {
        private readonly SoundEffect soundEffect;
        private readonly float volume;

        public SoundEffectWrapper(SoundEffect soundEffect, float volume)
        {
            this.volume = volume;
            this.soundEffect = soundEffect;
        }

        public void Play()
        {
            soundEffect.Play(volume, 0, 0);
        }

        public SoundEffect MonoGameSoundEffect()
        {
            return soundEffect;
        }
    }
}