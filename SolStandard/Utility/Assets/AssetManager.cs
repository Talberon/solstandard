using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite;
using SolStandard.Utility.Load;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public static class AssetManager
    {
        public static ISoundEffect MenuMoveSFX { get; private set; }
        public static ISoundEffect MenuConfirmSFX { get; private set; }
        public static ISoundEffect MapCursorMoveSFX { get; private set; }
        public static ISoundEffect MapUnitSelectSFX { get; private set; }
        public static ISoundEffect MapUnitCancelSFX { get; private set; }
        public static ISoundEffect MapUnitMoveSFX { get; private set; }
        public static ISoundEffect CombatStartSFX { get; private set; }
        public static ISoundEffect DiceRollSFX { get; private set; }
        public static ISoundEffect CombatBlockSFX { get; private set; }
        public static ISoundEffect CombatDamageSFX { get; private set; }
        public static ISoundEffect CombatDeathSFX { get; private set; }
        public static ISoundEffect DisableDiceSFX { get; private set; }
        public static ISoundEffect WarningSFX { get; private set; }
        public static ISoundEffect CoinSFX { get; private set; }
        public static ISoundEffect DoorSFX { get; private set; }
        public static ISoundEffect LockedSFX { get; private set; }
        public static ISoundEffect UnlockSFX { get; private set; }
        public static ISoundEffect DropItemSFX { get; private set; }
        public static ISoundEffect ErrorSFX { get; private set; }
        public static ISoundEffect PingSFX { get; private set; }
        public static ISoundEffect LogoSFX { get; private set; }

        public static ISoundEffect SkillBuffSFX { get; private set; }
        public static ISoundEffect SkillBlinkSFX { get; private set; }

        public static List<IPlayableAudio> MusicTracks { get; private set; }

        public static ITexture2D WindowTexture { get; private set; }
        private static List<ITexture2D> TerrainTextures { get; set; }
        public static List<ITexture2D> MapPreviewTextures { get; private set; }
        private static List<ITexture2D> GuiTextures { get; set; }

        private static List<ITexture2D> MiscIcons { get; set; }
        private static List<ITexture2D> ButtonIcons { get; set; }
        private static List<ITexture2D> KeyboardIcons { get; set; }
        private static List<ITexture2D> SkillIcons { get; set; }
        private static List<ITexture2D> StatusIcons { get; set; }
        private static List<ITexture2D> AnimationTextures { get; set; }
        private static List<ITexture2D> BannerIcons { get; set; }

        public static ITexture2D PistonTexture { get; private set; }
        public static ITexture2D SpringTexture { get; private set; }
        public static ITexture2D LockTexture { get; private set; }
        public static ITexture2D OpenTexture { get; private set; }
        
        public static AnimatedSprite DeveloperLogoSprite { get; private set; }
        public static ITexture2D SplashBackground { get; private set; }

        public static ITexture2D MainMenuLogoTexture { get; private set; }
        public static ITexture2D MainMenuSunTexture { get; private set; }
        public static ITexture2D MainMenuBackground { get; private set; }

        public static ISpriteFont WindowFont { get; private set; }
        public static ISpriteFont SmallWindowFont { get; private set; }
        public static ISpriteFont MapFont { get; private set; }
        public static ISpriteFont ResultsFont { get; private set; }
        public static ISpriteFont HeaderFont { get; private set; }
        public static ISpriteFont PromptFont { get; private set; }
        public static ISpriteFont MainMenuFont { get; private set; }
        public static ISpriteFont HeavyFont { get; private set; }
        public static ISpriteFont StatFont { get; private set; }

        public static ITexture2D ActionTiles { get; private set; }
        public static ITexture2D WhitePixel { get; private set; }
        public static ITexture2D WhiteGrid { get; private set; }
        public static ITexture2D DiceTexture { get; private set; }
        public static ITexture2D StatIcons { get; private set; }
        public static ITexture2D ObjectiveIcons { get; private set; }
        public static ITexture2D TeamIcons { get; private set; }

        public static List<ITexture2D> UnitSprites { get; private set; }
        public static List<ITexture2D> SmallPortraitTextures { get; private set; }

        public static string CreditsText { get; private set; }
        public static string EULAText { get; private set; }
        public static string HowToPlayText { get; private set; }

        public static ITexture2D EntitiesTexture
        {
            get { return TerrainTextures.Find(texture => texture.Name.Contains("Map/Tiles/entities-32")); }
        }

        public static ITexture2D OverworldTexture
        {
            get { return TerrainTextures.Find(texture => texture.Name.Contains("Map/Tiles/overworld-32")); }
        }

        public static ITexture2D MapCursorTexture
        {
            get
            {
                return GuiTextures.Find(texture =>
                    texture.MonoGameTexture.Name.Contains("Map/Cursor/Cursors"));
            }
        }

        public static ITexture2D MenuCursorTexture
        {
            get
            {
                return GuiTextures.Find(texture =>
                    texture.MonoGameTexture.Name.Contains("HUD/Cursor/MenuCursorPointer_32"));
            }
        }


        public static void LoadContent(ContentManager content)
        {
            DeveloperLogoSprite = ContentLoader.LoadDeveloperSplashLogoSprite(content);
            SplashBackground = ContentLoader.LoadDeveloperSplashBackgroundSprite(content);
        
            TerrainTextures = ContentLoader.LoadTerrainSpriteTexture(content);
            ActionTiles = ContentLoader.LoadActionTiles(content);

            WhitePixel = ContentLoader.LoadWhitePixel(content);
            WhiteGrid = ContentLoader.LoadWhiteGridOutline(content);

            StatIcons = ContentLoader.LoadStatIcons(content);
            UnitSprites = ContentLoader.LoadUnitSpriteTextures(content);

            GuiTextures = ContentLoader.LoadCursorTextures(content);
            WindowTexture = ContentLoader.LoadWindowTexture(content);
            MapPreviewTextures = ContentLoader.LoadMapPreviews(content);

            WindowFont = ContentLoader.LoadWindowFont(content);
            SmallWindowFont = ContentLoader.LoadSmallWindowFont(content);
            MapFont = ContentLoader.LoadMapFont(content);
            ResultsFont = ContentLoader.LoadResultsFont(content);
            HeaderFont = ContentLoader.LoadHeaderFont(content);
            PromptFont = ContentLoader.LoadPromptFont(content);
            HeavyFont = ContentLoader.LoadHeavyFont(content);
            StatFont = ContentLoader.LoadStatFont(content);

            PistonTexture = ContentLoader.LoadPistonAtlas(content);
            SpringTexture = ContentLoader.LoadLaunchpadAtlas(content);
            LockTexture = ContentLoader.LoadLockAtlas(content);
            OpenTexture = ContentLoader.LoadOpenAtlas(content);

            MainMenuFont = ContentLoader.LoadMainMenuFont(content);
            MainMenuLogoTexture = ContentLoader.LoadGameLogo(content);
            MainMenuSunTexture = ContentLoader.LoadSolIcon(content);
            MainMenuBackground = ContentLoader.LoadTitleScreenBackground(content);

            SmallPortraitTextures = ContentLoader.LoadSmallPortraits(content);

            DiceTexture = ContentLoader.LoadDiceAtlas(content);

            MiscIcons = ContentLoader.LoadMiscIcons(content);
            MiscIconProvider.LoadMiscIcons(MiscIcons);

            ButtonIcons = ContentLoader.LoadButtonIcons(content);
            ButtonIconProvider.LoadButtons(ButtonIcons);

            KeyboardIcons = ContentLoader.LoadKeyboardIcons(content);
            KeyboardIconProvider.LoadIcons(KeyboardIcons);

            SkillIcons = ContentLoader.LoadSkillIcons(content);
            SkillIconProvider.LoadSkillIcons(SkillIcons);

            StatusIcons = ContentLoader.LoadStatusIcons(content);
            StatusIconProvider.LoadStatusIcons(StatusIcons);

            AnimationTextures = ContentLoader.LoadAnimations(content);
            AnimatedSpriteProvider.LoadAnimatedSprites(AnimationTextures);
            
            BannerIcons = ContentLoader.LoadBannerIcons(content);
            BannerIconProvider.LoadBannerTextures(BannerIcons);

            ObjectiveIcons = ContentLoader.LoadObjectiveIcons(content);
            TeamIcons = ContentLoader.LoadTeamIcons(content);

            LogoSFX = ContentLoader.LoadLogoSFX(content);
            MenuMoveSFX = ContentLoader.LoadMenuMoveSFX(content);
            MenuConfirmSFX = ContentLoader.LoadMenuConfirmSFX(content);
            MapCursorMoveSFX = ContentLoader.LoadMapCursorMoveSFX(content);
            MapUnitSelectSFX = ContentLoader.LoadMapUnitSelectSFX(content);
            MapUnitCancelSFX = ContentLoader.LoadMapUnitCancelSFX(content);
            DiceRollSFX = ContentLoader.LoadDiceRollSFX(content);
            ErrorSFX = ContentLoader.LoadErrorSFX(content);

            CombatStartSFX = ContentLoader.LoadCombatStartSFX(content);
            MapUnitMoveSFX = ContentLoader.LoadMapUnitMoveSFX(content);
            CombatBlockSFX = ContentLoader.LoadCombatBlockSFX(content);
            CombatDamageSFX = ContentLoader.LoadCombatDamageSFX(content);
            CombatDeathSFX = ContentLoader.LoadCombatDeathSFX(content);
            DisableDiceSFX = ContentLoader.LoadDisableDiceSFX(content);
            WarningSFX = ContentLoader.LoadWarningSFX(content);
            CoinSFX = ContentLoader.LoadCoinSFX(content);
            DoorSFX = ContentLoader.LoadDoorSFX(content);
            LockedSFX = ContentLoader.LoadLockedSFX(content);
            UnlockSFX = ContentLoader.LoadUnlockSFX(content);
            DropItemSFX = ContentLoader.LoadDropItemSFX(content);
            PingSFX = ContentLoader.LoadPingSFX(content);

            SkillBuffSFX = ContentLoader.LoadSkillDrawSFX(content);
            SkillBlinkSFX = ContentLoader.LoadSkillBlinkSFX(content);

            MusicTracks = ContentLoader.LoadMusic(content);

            CreditsText = ContentLoader.LoadCreditsText();
            EULAText = ContentLoader.LoadEULAText();
            HowToPlayText = ContentLoader.LoadHowToPlayText();
        }
    }
}