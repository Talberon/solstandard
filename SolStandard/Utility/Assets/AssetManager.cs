using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using SolStandard.Utility.Load;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public static class AssetManager
    {
        private static List<ITexture2D> WindowTextures { get; set; }
        private static List<ITexture2D> TerrainTextures { get; set; }
        public static List<ITexture2D> MapPreviewTextures { get; private set; }
        private static List<ITexture2D> GuiTextures { get; set; }
        private static List<ITexture2D> ButtonIcons { get; set; }

        public static ITexture2D MainMenuLogoTexture { get; private set; }
        public static ITexture2D MainMenuSunTexture { get; private set; }
        public static ITexture2D MainMenuBackground { get; private set; }

        public static ISpriteFont WindowFont { get; private set; }
        public static ISpriteFont MapFont { get; private set; }
        public static ISpriteFont ResultsFont { get; private set; }
        public static ISpriteFont HeaderFont { get; private set; }
        public static ISpriteFont PromptFont { get; private set; }
        public static ISpriteFont MainMenuFont { get; private set; }

        public static ITexture2D ActionTiles { get; private set; }
        public static ITexture2D WhitePixel { get; private set; }
        public static ITexture2D WhiteGrid { get; private set; }
        public static ITexture2D DiceTexture { get; private set; }
        public static ITexture2D StatIcons { get; private set; }

        public static List<ITexture2D> UnitSprites { get; private set; }
        public static List<ITexture2D> LargePortraitTextures { get; private set; }
        public static List<ITexture2D> MediumPortraitTextures { get; private set; }
        public static List<ITexture2D> SmallPortraitTextures { get; private set; }

        public static ITexture2D WorldTileSetTexture
        {
            get { return TerrainTextures.Find(texture => texture.Name.Contains("Map/Tiles/WorldTileSet")); }
        }

        public static ITexture2D TerrainTexture
        {
            get { return TerrainTextures.Find(texture => texture.Name.Contains("Map/Tiles/Terrain")); }
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
                return GuiTextures.Find(texture => texture.MonoGameTexture.Name.Contains("HUD/Cursor/MenuCursorArrow"));
            }
        }

        public static ITexture2D WindowTexture
        {
            get { return WindowTextures.Find(texture => texture.MonoGameTexture.Name.Contains("LightWindow")); }
        }

        public static void LoadContent(ContentManager content)
        {
            TerrainTextures = ContentLoader.LoadTerrainSpriteTexture(content);
            ActionTiles = ContentLoader.LoadActionTiles(content);
            
            WhitePixel = ContentLoader.LoadWhitePixel(content);
            WhiteGrid = ContentLoader.LoadWhiteGridOutline(content);
            
            StatIcons = ContentLoader.LoadStatIcons(content);
            UnitSprites = ContentLoader.LoadUnitSpriteTextures(content);
            
            GuiTextures = ContentLoader.LoadCursorTextures(content);
            WindowTextures = ContentLoader.LoadWindowTextures(content);
            ButtonIcons = ContentLoader.LoadButtonIcons(content);
            MapPreviewTextures = ContentLoader.LoadMapPreviews(content);
            
            WindowFont = ContentLoader.LoadWindowFont(content);
            MapFont = ContentLoader.LoadMapFont(content);
            ResultsFont = ContentLoader.LoadResultsFont(content);
            HeaderFont = ContentLoader.LoadHeaderFont(content);
            PromptFont = ContentLoader.LoadPromptFont(content);
            
            MainMenuFont = ContentLoader.LoadMainMenuFont(content);
            MainMenuLogoTexture = ContentLoader.LoadGameLogo(content);
            MainMenuSunTexture = ContentLoader.LoadSolSpin(content);
            MainMenuBackground = ContentLoader.LoadTitleScreenBackground(content);
            
            LargePortraitTextures = ContentLoader.LoadLargePortraits(content);
            MediumPortraitTextures = ContentLoader.LoadMediumPortraits(content);
            SmallPortraitTextures = ContentLoader.LoadSmallPortraits(content);
            
            DiceTexture = ContentLoader.LoadDiceAtlas(content);

            ButtonIconProvider.LoadButtons(ButtonIcons);
        }
    }
}