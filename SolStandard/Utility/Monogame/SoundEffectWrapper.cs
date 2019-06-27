using Microsoft.Xna.Framework.Audio;

namespace SolStandard.Utility.Monogame
{
    public class SoundEffectWrapper : ISoundEffect
    {
        public static bool Muted { get; private set; }
        private readonly SoundEffect soundEffect;
        private readonly float volume;

        public SoundEffectWrapper(SoundEffect soundEffect, float volume)
        {
            this.volume = volume;
            this.soundEffect = soundEffect;
        }

        public static void ToggleMute()
        {
            Muted = !Muted;
        }

        public void Play()
        {
            if (!Muted)
            {
                soundEffect.Play(volume, 0, 0);
            }
        }

        SoundEffect ISoundEffect.MonoGameSoundEffect => soundEffect;
    }
}