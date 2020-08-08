using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using SolStandard.Map;

namespace Steelbreakers.Utility.Graphics.Particles
{
    public class SpriteParticle : AbstractParticleEffect
    {
        public override ParticleEffect Effect { get; }
        public override Layer Layer => Layer.OverlayEffect;
        public override float Height => 1;

        public SpriteParticle(SpriteAtlas sprite)
        {
            var originRegion = new TextureRegion2D(sprite.Texture.MonoGameTexture, sprite.SourceRectangle);
            Effect = GenerateEffect(originRegion, sprite.DefaultColor);
        }

        private static ParticleEffect GenerateEffect(TextureRegion2D textureRegion, Color particleColor)
        {
            return new ParticleEffect(name: "Sprite Particle", autoTrigger: false)
            {
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 400, TimeSpan.FromMilliseconds(500), Profile.Point())
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(-50, 50),
                            Mass = 1f,
                            Quantity = 1,
                            Rotation = 0f,
                            Color = particleColor.ToHsl()
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ScaleInterpolator
                                    {
                                        StartValue = new Vector2(1f),
                                        EndValue = new Vector2(0f)
                                    },
                                    new OpacityInterpolator
                                    {
                                        StartValue = 0.9f,
                                        EndValue = 0f
                                    },
                                }
                            },
                            new LinearGravityModifier
                            {
                                Direction = Vector2.UnitY,
                                Strength = 100f
                            }
                        }
                    }
                }
            };
        }
    }
}