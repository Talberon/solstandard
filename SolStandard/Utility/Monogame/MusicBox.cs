using Microsoft.Xna.Framework.Media;

namespace SolStandard.Utility.Monogame
{
    public static class MusicBox
    {
        public static bool Muted { get; private set; }
        private static Song _currentSong;
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

        public static void Play(Song song, float volume = 1f)
        {
            _currentSong = song;
            _currentVolume = volume;
            if (Muted) return;

            MediaPlayer.Play(song);
            MediaPlayer.Volume = volume;
            MediaPlayer.IsRepeating = false;
        }

        public static void PlayLoop(Song song, float volume = 1f)
        {
            _currentSong = song;
            _currentVolume = volume;
            if (Muted) return;

            MediaPlayer.Play(song);
            MediaPlayer.Volume = volume;
            MediaPlayer.IsRepeating = true;
        }

        public static void Stop()
        {
            MediaPlayer.Stop();
        }

        public static void Pause()
        {
            MediaPlayer.Pause();
        }
    }
}