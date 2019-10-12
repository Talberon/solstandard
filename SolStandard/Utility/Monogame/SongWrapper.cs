using Microsoft.Xna.Framework.Media;

namespace SolStandard.Utility.Monogame
{
    public class SongWrapper : IPlayableAudio
    {
        private readonly Song monogameSong;
        public float Volume { get; set; }
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