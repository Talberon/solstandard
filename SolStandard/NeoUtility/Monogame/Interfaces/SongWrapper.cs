using Microsoft.Xna.Framework.Media;

namespace Steelbreakers.Utility.Monogame.Interfaces
{
    public class SongWrapper : IPlayableAudio
    {
        private readonly Song monogameSong;
        private float volume;

        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                MediaPlayer.Volume = volume;
            }
        }

        public string Name => monogameSong.Name;

        public SongWrapper(Song monogameSong)
        {
            this.monogameSong = monogameSong;
        }

        public void PlayLoop()
        {
            MediaPlayer.Volume = Volume;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(monogameSong);
        }
        
        public void PlayOnce()
        {
            MediaPlayer.Volume = Volume;
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Play(monogameSong);
        }

        public void Stop()
        {
            MediaPlayer.Stop();
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }
    }
}