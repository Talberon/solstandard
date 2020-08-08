using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using SolStandard;
using SolStandard.Utility.Exceptions;

namespace Steelbreakers.Utility.Monogame.Interfaces
{
    public class SoundEffectWrapper : ISoundEffect
    {
        private static bool Muted { get; set; }
        private readonly SoundEffect monogameSfx;
        private readonly float variance;
        public float Volume { get; set; }
        public SoundEffectInstance? Instance { get; private set; }

        SoundEffect ISoundEffect.MonoGameSoundEffect => monogameSfx;
        public string Name => monogameSfx.Name;

        public SoundEffectWrapper(SoundEffect monogameSfx, float volume, float variance)
        {
            Volume = volume;
            this.monogameSfx = monogameSfx;
            ZeroOneRangeException.Assert(variance);
            this.variance = variance;
        }

        public static void ToggleMute()
        {
            Muted = !Muted;
        }

        public void Play()
        {
            if (!Muted)
            {
                monogameSfx.Play(Volume, GameDriver.Random.NextSingle(-variance, variance), 0);
            }
        }

        public void PlayOnce()
        {
            Play();
        }

        public void PlayLoop()
        {
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }

            Instance = monogameSfx.CreateInstance();
            Instance!.IsLooped = true;
            Instance.Volume = Volume;
            Instance.Play();
        }

        public void Pause()
        {
            Instance?.Pause();
        }

        public void Stop()
        {
            Instance?.Stop();
        }
    }
}