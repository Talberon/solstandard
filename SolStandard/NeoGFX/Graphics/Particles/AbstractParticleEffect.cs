using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using SolStandard.Map;

namespace SolStandard.NeoGFX.Graphics.Particles
{
    public abstract class AbstractParticleEffect
    {
        public abstract ParticleEffect Effect { get; }
        public abstract Layer Layer { get; }
        public abstract float Height { get; }

        public void Trigger(Vector2 mapCoordinates)
        {
            Effect.Position = -Vector2.One * 1000;
            Effect.Trigger(
                mapCoordinates,
                SpriteBatchExtensions.GetLayerDepth(mapCoordinates.Y, Height, Layer)
            );
        }

        public void Update(float elapsedSeconds) => Effect.Update(elapsedSeconds);
    }
}