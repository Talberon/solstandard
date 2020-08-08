using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using SolStandard.Map;
using SolStandard.NeoUtility.Monogame.Assets;

namespace SolStandard.NeoGFX.Graphics.Particles
{
    public class DustEffect : AbstractParticleEffect
    {
        private readonly float radius;
        private readonly int particleEmitRate;
        public override ParticleEffect Effect { get; }
        public override Layer Layer => Layer.OverlayEffect;
        public override float Height => radius * 2;

        public DustEffect(int particleEmitRate = 1, float radius = 1f)
        {
            this.particleEmitRate = particleEmitRate;
            this.radius = radius;
            Texture2D texture = AssetManager.DustParticle.MonoGameTexture;
            Effect = GenerateEffect(new TextureRegion2D(texture));
        }

        private ParticleEffect GenerateEffect(TextureRegion2D textureRegion)
        {
            return new ParticleEffect(name: "Dust", autoTrigger: false)
            {
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromMilliseconds(200),
                        Profile.Ring(radius, Profile.CircleRadiation.Out))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 10f),
                            Quantity = particleEmitRate,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(0.1f, 0.4f)
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ColorInterpolator
                                    {
                                        StartValue = new HslColor(.19f, .85f, .86f),
                                        EndValue = new HslColor(0.5f, 0.9f, 1.0f)
                                    }
                                }
                            },
                            new RotationModifier {RotationRate = -2.1f},
                            new RectangleContainerModifier {Width = 800, Height = 480},
                            new LinearGravityModifier {Strength = 30f}
                        }
                    }
                }
            };
        }
    }
}