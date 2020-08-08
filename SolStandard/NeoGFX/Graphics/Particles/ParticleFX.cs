using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;

namespace SolStandard.NeoGFX.Graphics.Particles
{
    public class ParticleFx : IRenderable
    {
        public float Width => 1;
        public float Height => 1;

        public TimeSpan EmitterLifespan => abstractParticleEffect.Effect.Emitters.First().LifeSpan;

        private TimeSpan activeEmitTime;
        private bool IsEmitting => activeEmitTime > TimeSpan.Zero;

        private readonly AbstractParticleEffect abstractParticleEffect;

        public ParticleFx(AbstractParticleEffect abstractParticleEffect)
        {
            this.abstractParticleEffect = abstractParticleEffect;
            activeEmitTime = TimeSpan.Zero;
        }

        public void PlayEffectAtLocation(Vector2 mapCoordinates)
        {
            abstractParticleEffect.Trigger(mapCoordinates);
            activeEmitTime = EmitterLifespan;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsEmitting) return;
            activeEmitTime -= gameTime.ElapsedGameTime;
            abstractParticleEffect.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsEmitting) return;
            spriteBatch.Draw(abstractParticleEffect.Effect);
        }
    }
}