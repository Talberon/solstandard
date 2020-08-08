using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using SolStandard;
using SolStandard.Map;
using Steelbreakers.Utility.Monogame.Assets;

namespace Steelbreakers.Utility.Graphics.Particles
{
    public class DebrisEffect : AbstractParticleEffect
    {
        private readonly Color startColor;
        public override ParticleEffect Effect { get; }
        public override Layer Layer => Layer.OverlayEffect;
        public override float Height => GameDriver.CellSizeFloat / 2;

        public DebrisEffect(Color startColor)
        {
            this.startColor = startColor;
            Texture2D texture = AssetManager.DebrisParticle.MonoGameTexture;
            Effect = GenerateEffect(new TextureRegion2D(texture));
        }

        private ParticleEffect GenerateEffect(TextureRegion2D textureRegion)
        {
            return new ParticleEffect(name: "Debris", autoTrigger: false)
            {
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromMilliseconds(600),
                        Profile.BoxFill(GameDriver.CellSizeFloat / 2, Height))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 60f),
                            Quantity = 20,
                            Rotation = new Range<float>(-1f, 1f),
                            Mass = 0.9f,
                            Scale = new Range<float>(0.6f, 0.8f),
                            Color = startColor.ToHsl(),
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ScaleInterpolator
                                    {
                                        StartValue = new Vector2(0.8f),
                                        EndValue = new Vector2(0.1f)
                                    },
                                    new OpacityInterpolator
                                    {
                                        StartValue = 1f,
                                        EndValue = 0f
                                    },
                                }
                            },
                            new RotationModifier {RotationRate = -2.1f},
                            new RectangleContainerModifier {Width = 800, Height = 480},
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 10f}
                        }
                    }
                }
            };
        }
    }
}