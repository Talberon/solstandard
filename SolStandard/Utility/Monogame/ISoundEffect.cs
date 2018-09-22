using Microsoft.Xna.Framework.Audio;

namespace SolStandard.Utility.Monogame
{
    public interface ISoundEffect
    {
        void Play();
        SoundEffect MonoGameSoundEffect();
    }
}