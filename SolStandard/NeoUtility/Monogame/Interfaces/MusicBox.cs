using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using SolStandard;
using Steelbreakers.Utility.Monogame.Assets;

namespace Steelbreakers.Utility.Monogame.Interfaces
{
    public static class MusicBox
    {
        private const float MaxVolume = 1f;
        private const float MinVolume = 0f;
        private const string SaveFileName = "musicvolume";
        public static bool Muted { get; private set; }
        private static IPlayableAudio? _currentSong;
        public static float CurrentVolume { get; private set; } = LoadVolume();

        public enum Songs
        {
            MainTheme,
            Victory,
            Plains,
            Sol
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private static Dictionary<Songs, IPlayableAudio> AvailableSongs { get; set; }

        public static void LoadContent()
        {
            AvailableSongs = new Dictionary<Songs, IPlayableAudio>
            {
                [Songs.MainTheme] = ContentLoader.LoadBGM("theme-song"),
                [Songs.Victory] = ContentLoader.LoadBGM("victory"),
                [Songs.Plains] = ContentLoader.LoadBGM("skirmish-on-the-plains"),
                [Songs.Sol] = ContentLoader.LoadBGM("sol-standard-battle-remix")
            };
        }

        public static void ToggleMute()
        {
            Muted = !Muted;

            if (Muted)
            {
                MediaPlayer.Pause();
                _currentSong?.Pause();
            }
            else
            {
                if (_currentSong is object) PlayLoop(_currentSong);
            }
        }

        private static void PlayOnce(IPlayableAudio song)
        {
            _currentSong?.Stop();

            _currentSong = song;
            if (Muted) return;

            song.Volume = CurrentVolume;
            song.PlayOnce();
        }

        public static void PlayOnce(Songs song)
        {
            PlayOnce(AvailableSongs[song]);
        }

        private static void PlayLoop(IPlayableAudio song)
        {
            _currentSong?.Stop();

            _currentSong = song;
            if (Muted) return;

            song.Volume = CurrentVolume;
            song.PlayLoop();
        }

        public static void PlayLoop(Songs song)
        {
            PlayLoop(AvailableSongs[song]);
        }

        public static void IncreaseVolumeBy(float increasedBy)
        {
            if (CurrentVolume + increasedBy > MaxVolume)
            {
                CurrentVolume = MaxVolume;
            }
            else
            {
                CurrentVolume += increasedBy;
            }

            UpdateVolume();
            SaveVolume();
        }

        public static void ReduceVolumeBy(float reducedBy)
        {
            if (CurrentVolume - reducedBy < MinVolume)
            {
                CurrentVolume = MinVolume;
            }
            else
            {
                CurrentVolume -= reducedBy;
            }

            UpdateVolume();
            SaveVolume();
        }

        private static void UpdateVolume()
        {
            MediaPlayer.Volume = CurrentVolume;
            if (_currentSong is SoundEffectWrapper songEffect && songEffect.Instance is { } songInstance)
            {
                songInstance.Volume = CurrentVolume;
            }

            foreach (IPlayableAudio availableSongsValue in AvailableSongs.Values)
            {
                availableSongsValue.Volume = CurrentVolume;
            }
        }


        private static float LoadVolume()
        {
            const float defaultVolume = 0.7f;

            return GameDriver.FileIO.FileExists(SaveFileName)
                ? GameDriver.FileIO.Load<float>(SaveFileName)
                : defaultVolume;
        }

        private static void SaveVolume()
        {
            GameDriver.FileIO.Save(SaveFileName, CurrentVolume);
        }

        public static void Pause()
        {
            _currentSong?.Pause();
        }
    }
}