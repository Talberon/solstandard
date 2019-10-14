namespace SolStandard.Utility.Monogame
{
    public interface IPlayableAudio
    {
        string Name { get; }
        float Volume { get; set; }
        void PlayOnce();
        void PlayLoop();
        void Pause();
        void Stop();
    }
}