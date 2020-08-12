using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using MonoGame.Extended;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Collections;
using SolStandard.Utility.HUD.Directions;
using SolStandard.Utility.HUD.Juice;
using SolStandard.Utility.HUD.Neo;
using SolStandard.Utility.HUD.Sound;
using SolStandard.Utility.HUD.Sprite;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Components.SplashScreen
{
    public class SplashScreenHUD : IUserInterface, IUpdate
    {
        private readonly ScrollingBackground scrollingBackground;
        public List<NeoWindow> Windows { get; }
        public float Width => GameDriver.RenderResolution.X;
        public float Height => GameDriver.RenderResolution.Y;

        private enum Phase
        {
            FadeIn,
            PlayAnimation,
            FadeOut,
            Exit
        }

        private readonly TimeSpanStateQueue<Phase> phaseDirector;
        private readonly AnimatedSprite logoSprite;
        private readonly JuiceBox logoJuiceBox;
        private static readonly Color ScrollingBackgroundColor = new Color(150, 150, 150);
        private readonly SoundEffectPlayer soundEffectPlayer;

        public SplashScreenHUD()
        {
            phaseDirector = new TimeSpanStateQueue<Phase>(Phase.Exit);
            phaseDirector.PushStateForDuration(Phase.FadeIn, TimeSpan.FromSeconds(0.8f));
            phaseDirector.PushStateForDuration(Phase.PlayAnimation, TimeSpan.FromSeconds(2.1f));
            phaseDirector.PushStateForDuration(Phase.FadeOut, TimeSpan.FromSeconds(0.8f));

            const float fadeSpeed = 0.7f;

            SpriteAtlas singleImageSprite = AssetManager.SplashBackground
                .ToSingleImageSprite();
            const float scaleFactor = 15f;
            scrollingBackground = new ScrollingBackground(
                (singleImageSprite.Resize(
                        new Vector2(singleImageSprite.Width, singleImageSprite.Height) * scaleFactor
                    ) as SpriteAtlas
                )
                .WithColor(new Color(Color.Black, 0f)),
                IntercardinalDirection.SouthEast,
                1f,
                fadeSpeed
            );

            logoSprite = AssetManager.DeveloperLogoSprite;
            logoSprite.Animating = false;
            logoSprite.RenderDefinition.Color = Color.TransparentBlack;
            logoSprite.RenderDefinition.Scale = Vector2.One * 10f;
            logoJuiceBox = new JuiceBox.Builder(fadeSpeed).WithColorShifting(Color.Transparent).Build();

            Windows = new List<NeoWindow>
            {
                logoSprite.ToWrapper().ToWindowBuilder().WindowColor(Color.Transparent)
                    .CenterOf(GameDriver.VirtualBounds).Build()
            };

            soundEffectPlayer = new SoundEffectPlayer(AssetManager.LogoSFX, TimeSpan.FromMinutes(1));
        }

        public void Update(GameTime gameTime)
        {
            soundEffectPlayer.Update(gameTime);
            phaseDirector.Update(gameTime);
            scrollingBackground.Update(gameTime);
            Windows.ForEach(window => window.Update(gameTime));

            logoJuiceBox.Update();
            logoSprite.RenderDefinition.Color = logoJuiceBox.CurrentColor;

            switch (phaseDirector.CurrentState)
            {
                case Phase.FadeIn:
                    FadeIn();
                    break;
                case Phase.PlayAnimation:
                    PlayAnimation();
                    break;
                case Phase.FadeOut:
                    FadeOut();
                    break;
                case Phase.Exit:
                    GlobalContext.CurrentGameState = GlobalContext.EULAContext.EULAConfirmed
                        ? GlobalContext.GameState.MainMenu
                        : GlobalContext.GameState.EULAConfirm;

                    GlobalContext.CenterCursorAndCamera();

                    MusicBox.PlayLoop(AssetManager.MusicTracks.Find(track => track.Name.EndsWith("MapSelectTheme")));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FadeIn()
        {
            logoJuiceBox.HueShiftTo(Color.White);
            scrollingBackground.HueShiftTowards(ScrollingBackgroundColor);
        }

        private void PlayAnimation()
        {
            logoSprite.Animating = true;
            logoSprite.PlayOnceThenFreeze("splash");

            GlobalAsyncActions.PerformActionAfterTime(soundEffectPlayer.Play, TimeSpan.FromMilliseconds(100));
        }

        private void FadeOut()
        {
            logoJuiceBox.HueShiftTo(Color.TransparentBlack);
            scrollingBackground.HueShiftTowards(Color.TransparentBlack);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            scrollingBackground.Draw(spriteBatch);
            Windows.ForEach(window => window.Draw(spriteBatch));
        }
    }
}