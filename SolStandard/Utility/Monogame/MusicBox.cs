using Microsoft.Xna.Framework.Media;

namespace SolStandard.Utility.Monogame
{
    public static class MusicBox
    {
        private const float DefaultVolume = 0.7f;
        private const float MaxVolume = 1f;
        private const float MinVolume = 0f;
        public static bool Muted { get; private set; }
        private static IPlayableAudio _currentSong;
        private static float _currentVolume;

        public static void ToggleMute()
        {
            Muted = !Muted;

            if (Muted)
            {
                MediaPlayer.Pause();
            }
            else
            {
                Play(_currentSong, _currentVolume);
            }
        }

        public static void Play(IPlayableAudio song, float volume = DefaultVolume)
        {
            _currentSong?.Stop();
            
            _currentSong = song;
            _currentVolume = volume;
            if (Muted) return;

            song.Volume = _currentVolume;
            song.PlayOnce();
        }

        public static void PlayLoop(IPlayableAudio song, float volume = DefaultVolume)
        {
            _currentSong?.Stop();
            
            _currentSong = song;
            _currentVolume = volume;
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
        }

        public static void Pause()
        {
            _currentSong?.Pause();
        }
    }
}