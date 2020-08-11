using Microsoft.Xna.Framework.Audio;

namespace SolStandard.Utility.Monogame
{
    public class SoundEffectWrapper : ISoundEffect
    {
        public static bool Muted { get; private set; }
        private readonly SoundEffect monogameSfx;
        public float Volume { get; set; }
        private SoundEffectInstance sfxInstance;

        SoundEffect ISoundEffect.MonoGameSoundEffect => monogameSfx;
        public string Name => monogameSfx.Name;

        public SoundEffectWrapper(SoundEffect monogameSfx, float volume)
        {
            Volume = volume;
            this.monogameSfx = monogameSfx;
            Muted = LoadMuted();
        }

        public static void ToggleMute()
        {
            Muted = !Muted;
            SaveMuted();
        }

        public void Play()
        {
            if (!Muted)
            {
                monogameSfx.Play(Volume, 0, 0);
            }
        }

        public void PlayOnce()
        {
            Play();
        }

        public void PlayLoop()
        {
            if (sfxInstance != null)
            {
                sfxInstance.Dispose();
                sfxInstance = null;
            }

            sfxInstance = monogameSfx.CreateInstance();
            sfxInstance.IsLooped = true;
            sfxInstance.Volume = Volume;
            sfxInstance.Play();
        }

        public void Pause()
        {
            sfxInstance?.Pause();
        }

        public void Stop()
        {
            sfxInstance?.Stop();
        }

        private const string SaveFileName = "soundmuted";

        private static void SaveMuted()
        {
            GameDriver.FileIO.Save(SaveFileName, Muted);
        }

        private static bool LoadMuted()
        {
            return GameDriver.FileIO.FileExists(SaveFileName) && GameDriver.FileIO.Load<bool>(SaveFileName);
        }
    }
}