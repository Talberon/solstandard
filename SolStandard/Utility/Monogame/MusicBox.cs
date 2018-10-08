using Microsoft.Xna.Framework.Media;

namespace SolStandard.Utility.Monogame
{
    public static class MusicBox
    {
        public static void Play(Song song)
        {
            MediaPlayer.Play(song);
            MediaPlayer.Volume = 1;
            MediaPlayer.IsRepeating = false;
        }

        public static void Play(Song song, float volume)
        {
            MediaPlayer.Play(song);
            MediaPlayer.Volume = volume;
            MediaPlayer.IsRepeating = false;
        }

        public static void PlayLoop(Song song)
        {
            MediaPlayer.Play(song);
            MediaPlayer.Volume = 1;
            MediaPlayer.IsRepeating = true;
        }

        public static void PlayLoop(Song song, float volume)
        {
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