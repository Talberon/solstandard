using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.HUD.Sound
{
    public class SoundEffectPlayer : IUpdate, ISoundEffect
    {
        public SoundEffect MonoGameSoundEffect => SoundEffect.MonoGameSoundEffect;
        private ISoundEffect SoundEffect { get; }
        public string Name => SoundEffect.Name;

        public float Volume
        {
            get => SoundEffect.Volume;
            set => SoundEffect.Volume = value;
        }

        private readonly TimeSpan cooldownPeriod;
        private TimeSpan Timer { get; set; }
        private bool IsOnCooldown => Timer > TimeSpan.Zero;

        public SoundEffectPlayer(ISoundEffect soundEffect, TimeSpan cooldownPeriod)
        {
            SoundEffect = soundEffect;
            this.cooldownPeriod = cooldownPeriod;
            Timer = TimeSpan.Zero;
        }

        public void Update(GameTime gameTime)
        {
            if (IsOnCooldown) Timer -= gameTime.ElapsedGameTime;
        }

        public void Play()
        {
            if (IsOnCooldown) return;
            Timer = cooldownPeriod;
            SoundEffect.Play();
        }

        public void PlayOnce()
        {
            if (IsOnCooldown) return;
            Timer = cooldownPeriod;
            SoundEffect.PlayOnce();
        }

        public void PlayLoop()
        {
            if (IsOnCooldown) return;
            Timer = cooldownPeriod;
            SoundEffect.PlayLoop();
        }

        public void Pause()
        {
            SoundEffect.PlayOnce();
        }

        public void Stop()
        {
            SoundEffect.PlayOnce();
        }
    }
}