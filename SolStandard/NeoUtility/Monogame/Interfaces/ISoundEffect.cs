﻿using Microsoft.Xna.Framework.Audio;

 namespace Steelbreakers.Utility.Monogame.Interfaces
{
    public interface ISoundEffect : IPlayableAudio
    {
        void Play();
        SoundEffect MonoGameSoundEffect { get; }
    }
}