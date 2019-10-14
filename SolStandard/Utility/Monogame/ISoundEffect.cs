using Microsoft.Xna.Framework.Audio;

namespace SolStandard.Utility.Monogame
{
    public interface ISoundEffect : IPlayableAudio
    {
        void Play();
        SoundEffect MonoGameSoundEffect { get; }
    }
}