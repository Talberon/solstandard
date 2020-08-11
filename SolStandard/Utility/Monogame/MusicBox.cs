using Microsoft.Xna.Framework.Media;

namespace SolStandard.Utility.Monogame
{
    public static class MusicBox
    {
        private const float MaxVolume = 1f;
        private const float MinVolume = 0f;
        public static bool Muted { get; private set; } = LoadMuted();
        private static IPlayableAudio _currentSong;
        private static float _currentVolume = LoadVolume();

        public static void ToggleMute()
        {
            Muted = !Muted;
            SaveMuted();

            if (Muted)
            {
                MediaPlayer.Pause();
            }
            else
            {
                PlayLoop(_currentSong);
            }
        }

        public static void PlayOnce(IPlayableAudio song)
        {
            MediaPlayer.Volume = _currentVolume;
            _currentSong?.Stop();

            _currentSong = song;
            if (Muted) return;

            song.Volume = _currentVolume;
            song.PlayOnce();
        }

        public static void PlayLoop(IPlayableAudio song)
        {
            MediaPlayer.Volume = _currentVolume;
            _currentSong?.Stop();

            _currentSong = song;
            if (Muted) return;

            song.Volume = _currentVolume;
            song.PlayLoop();
        }

        public static void IncreaseVolume(float increasedBy)
        {
            if (_currentVolume + increasedBy > MaxVolume)
            {
                _currentVolume = MaxVolume;
            }
            else
            {
                _currentVolume += increasedBy;
            }

            MediaPlayer.Volume = _currentVolume;
            SaveVolume();
        }

        public static void ReduceVolume(float reducedBy)
        {
            if (_currentVolume - reducedBy < MinVolume)
            {
                _currentVolume = MinVolume;
            }
            else
            {
                _currentVolume -= reducedBy;
            }

            MediaPlayer.Volume = _currentVolume;
            SaveVolume();
        }

        public static void Pause()
        {
            _currentSong?.Pause();
        }


        private const string SaveFileName = "musicvolume";

        private static void SaveVolume()
        {
            GameDriver.FileIO.Save(SaveFileName, _currentVolume);
        }

        private static float LoadVolume()
        {
            const float defaultVolume = 0.7f;

            return GameDriver.FileIO.FileExists(SaveFileName)
                ? GameDriver.FileIO.Load<float>(SaveFileName)
                : defaultVolume;
        }

        private const string MutedFileName = "musicmuted";

        private static void SaveMuted()
        {
            GameDriver.FileIO.Save(MutedFileName, Muted);
        }

        private static bool LoadMuted()
        {
            return GameDriver.FileIO.FileExists(MutedFileName) && GameDriver.FileIO.Load<bool>(MutedFileName);
        }
    }
}