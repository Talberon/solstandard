using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using Steelbreakers.Utility.Graphics;
using Steelbreakers.Utility.Monogame.Interfaces;

// ReSharper disable NotNullMemberIsNotInitialized

namespace Steelbreakers.Utility.Monogame.Assets
{
    public static class AssetManager
    {
        public static GraphicsDevice GraphicsDevice;

        public static AnimatedSprite PlayerOneSprite => ContentLoader.LoadRedPlayerSprite();
        public static AnimatedSprite PlayerTwoSprite => ContentLoader.LoadBluePlayerSprite();
        public static AnimatedSprite PlayerThreeSprite => ContentLoader.LoadGreenPlayerSprite();
        public static AnimatedSprite PlayerFourSprite => ContentLoader.LoadYellowPlayerSprite();

        private static AnimatedSprite PlayerRedPortrait => ContentLoader.LoadRedPlayerPortrait();
        private static AnimatedSprite PlayerBluePortrait => ContentLoader.LoadBluePlayerPortrait();
        private static AnimatedSprite PlayerGreenPortrait => ContentLoader.LoadGreenPlayerPortrait();
        private static AnimatedSprite PlayerYellowPortrait => ContentLoader.LoadYellowPlayerPortrait();

        public static AnimatedSprite GreenSlimeSprite => ContentLoader.LoadSlimeSprite("green");
        public static AnimatedSprite RedSlimeSprite => ContentLoader.LoadSlimeSprite("red");
        public static AnimatedSprite BlueSlimeSprite => ContentLoader.LoadSlimeSprite("blue");

        public static AnimatedSprite GreenOrcSprite => ContentLoader.LoadOrcSprite("green");

        public static AnimatedSprite ArrowSprite => ContentLoader.LoadArrowSprite();
        public static AnimatedSprite JavelinSprite => ContentLoader.LoadJavelinSprite();

        public static AnimatedSprite CombatHudSprite => ContentLoader.LoadCombatHudSprite();
        public static AnimatedSprite StatefulObjectsSprite => ContentLoader.LoadStatefulObjectsSprite();
        public static AnimatedSprite ObjectiveIconsSprite => ContentLoader.LoadObjectiveIconsSprite();

        public static AnimatedSprite ItemsSprite => ContentLoader.LoadItemsSprite();
        public static AnimatedSprite HeartPieceSprite => ContentLoader.LoadHeartPieceSprite();
        public static AnimatedSprite ArmorPieceSprite => ContentLoader.LoadArmorPieceSprite();

        public static AnimatedSprite LogoSprite => ContentLoader.LoadLogoSprite();
        public static AnimatedSprite DeveloperLogoSprite => ContentLoader.LoadDeveloperSplashLogoSprite();

        public static ITexture2D TitleScreenBackground { get; private set; }
        public static ITexture2D WeaponBackground { get; private set; }
        public static ITexture2D RandomBackground { get; private set; }
        public static ITexture2D RandomIcon { get; private set; }

        public static ISpriteFont WindowFont { get; private set; }
        public static ISpriteFont TinyFont { get; private set; }
        public static ISpriteFont TitleFont { get; private set; }

        public static ITexture2D RoundedWindowTexture { get; private set; }
        public static ITexture2D SharpWindowTexture { get; private set; }

        public static ITexture2D WhitePixel { get; private set; }

        public static ITexture2D DustParticle { get; private set; }
        public static ITexture2D SkullParticle { get; private set; }
        public static ITexture2D DebrisParticle { get; private set; }
        public static ITexture2D SparkleParticle { get; private set; }
        public static ITexture2D ArmorParticle { get; private set; }
        public static ITexture2D HeartParticle { get; private set; }

        public static ITexture2D SolIconHiRes { get; private set; }

        private static ITexture2D Spotlight { get; set; }
        private static ITexture2D ShadowParticle { get; set; }
        private static ITexture2D SmallShadowParticle { get; set; }

        public static SpriteAtlas ShadowSprite => ShadowParticle.ToSingleImageSprite();
        public static SpriteAtlas SmallShadowSprite => SmallShadowParticle.ToSingleImageSprite();
        public static SpriteAtlas SpotlightSprite => Spotlight.ToSingleImageSprite(layerDepth: 2);

        public static SpriteAtlas GetMapPreviewImage(string mapName)
        {
            return ContentLoader.LoadMapPreviewImage(mapName).ToSingleImageSprite();
        }

        public static void LoadContent(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            TitleScreenBackground = ContentLoader.LoadTitleScreenBackground();
            WeaponBackground = ContentLoader.LoadWeaponBackground();
            RandomBackground = ContentLoader.LoadRandomBackground();
            RandomIcon = ContentLoader.LoadRandomIcon();

            RoundedWindowTexture = ContentLoader.LoadRoundedWindowTexture();
            SharpWindowTexture = ContentLoader.LoadSharpWindowTexture();
            WhitePixel = ContentLoader.LoadWhitePixel();
            DustParticle = ContentLoader.LoadDustParticle();
            SkullParticle = ContentLoader.LoadSkullParticle();
            DebrisParticle = ContentLoader.LoadDebrisParticle();
            SparkleParticle = ContentLoader.LoadSparkleParticle();
            ArmorParticle = ContentLoader.LoadShieldParticle();
            HeartParticle = ContentLoader.LoadHeartParticle();
            Spotlight = ContentLoader.LoadSpotlightTexture();

            ShadowParticle = ContentLoader.LoadShadowParticle();
            SmallShadowParticle = ContentLoader.LoadSmallShadowParticle();

            SolIconHiRes = ContentLoader.LoadSolIconHiRes();

            WindowFont = ContentLoader.LoadWindowFont();
            TinyFont = ContentLoader.LoadTinyFont();
            TitleFont = ContentLoader.LoadTitleFont();

            GamepadIconProvider.LoadIcons(ContentLoader.LoadGamepadIcons());
            KeyboardIconProvider.LoadIcons(ContentLoader.LoadKeyboardIcons());
        }

        public static ITexture2D WhiteOutTexture(Texture2D textureToWhiteOut)
        {
            var pixels = new Color[textureToWhiteOut.Width * textureToWhiteOut.Height];
            textureToWhiteOut.GetData(pixels);

            for (int index = 0; index < pixels.Length; index++)
            {
                if (pixels[index].A != decimal.Zero)
                {
                    pixels[index] = Color.White;
                }
            }

            var whiteTexture = new Texture2D(GraphicsDevice, textureToWhiteOut.Width, textureToWhiteOut.Height);
            whiteTexture.SetData(pixels);

            return new Texture2DWrapper(whiteTexture);
        }
    }
}