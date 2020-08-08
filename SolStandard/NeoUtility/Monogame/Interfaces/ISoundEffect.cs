﻿using Microsoft.Xna.Framework.Audio;

 namespace SolStandard.NeoUtility.Monogame.Interfaces
{
    public interface ISoundEffect : IPlayableAudio
    {
        void Play();
        SoundEffect MonoGameSoundEffect { get; }
    }
}