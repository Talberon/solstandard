using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace SolStandard.Utility.Load
{
    using Monogame;

    /**
     * ContentLoader
     * Holds a series of loader methods that are used for the game
     */
    public static class ContentLoader
    {
        public static ISpriteFont LoadPromptFont(ContentManager content)
        {
            return new SpriteFontWrapper(content.Load<SpriteFont>("Fonts/PromptText"));
        }

        public static ISpriteFont LoadHeaderFont(ContentManager content)
        {
            return new SpriteFontWrapper(content.Load<SpriteFont>("Fonts/WindowHeaderText"));
        }

        public static ISpriteFont LoadWindowFont(ContentManager content)
        {
            return new SpriteFontWrapper(content.Load<SpriteFont>("Fonts/WindowText"));
        }

        public static ISpriteFont LoadMapFont(ContentManager content)
        {
            return new SpriteFontWrapper(content.Load<SpriteFont>("Fonts/MapText"));
        }

        public static ISpriteFont LoadResultsFont(ContentManager content)
        {
            return new SpriteFontWrapper(content.Load<SpriteFont>("Fonts/ResultsText"));
        }

        public static ISpriteFont LoadMainMenuFont(ContentManager content)
        {
            return new SpriteFontWrapper(content.Load<SpriteFont>("Fonts/MainMenuText"));
        }

        public static ITexture2D LoadWhitePixel(ContentManager content)
        {
            Texture2D spriteTextures = content.Load<Texture2D>("Graphics/WhitePixel");

            return new Texture2DWrapper(spriteTextures);
        }

        public static ITexture2D LoadWhiteGridOutline(ContentManager content)
        {
            Texture2D spriteTextures = content.Load<Texture2D>("Graphics/Map/Tiles/GridOutline");

            return new Texture2DWrapper(spriteTextures);
        }

        public static List<ITexture2D> LoadTerrainSpriteTexture(ContentManager content)
        {
            List<Texture2D> loadTerrainTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Map/Tiles/Tiles"),
                content.Load<Texture2D>("Graphics/Map/Tiles/Terrain"),
                content.Load<Texture2D>("Graphics/Map/Tiles/WorldTileSet")
            };

            List<ITexture2D> terrainTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadTerrainTextures)
            {
                terrainTextures.Add(new Texture2DWrapper(texture));
            }

            return terrainTextures;
        }

        public static ITexture2D LoadActionTiles(ContentManager content)
        {
            Texture2D actionTilesTexture = content.Load<Texture2D>("Graphics/Map/Tiles/ActionTiles");

            ITexture2D actionTiles = new Texture2DWrapper(actionTilesTexture);

            return actionTiles;
        }

        public static ITexture2D LoadStatIcons(ContentManager content)
        {
            Texture2D statIconsTexture = content.Load<Texture2D>("Graphics/Images/Icons/StatIcons");

            ITexture2D statIcons = new Texture2DWrapper(statIconsTexture);

            return statIcons;
        }

        public static List<ITexture2D> LoadCursorTextures(ContentManager content)
        {
            List<Texture2D> loadCursorTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Map/Cursor/Cursors"),
                content.Load<Texture2D>("Graphics/HUD/Cursor/MenuCursorArrow_32")
            };

            List<ITexture2D> cursorTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadCursorTextures)
            {
                cursorTextures.Add(new Texture2DWrapper(texture));
            }

            return cursorTextures;
        }

        public static List<ITexture2D> LoadWindowTextures(ContentManager content)
        {
            List<Texture2D> loadWindowTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/HUD/Window/GreyWindow"),
                content.Load<Texture2D>("Graphics/HUD/Window/LightWindow")
            };

            List<ITexture2D> windowTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadWindowTextures)
            {
                windowTextures.Add(new Texture2DWrapper(texture));
            }

            return windowTextures;
        }

        public static List<ITexture2D> LoadUnitSpriteTextures(ContentManager content)
        {
            List<Texture2D> loadSpriteTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueArcher"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueMage"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueChampion"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueMonarch"),

                content.Load<Texture2D>("Graphics/Map/Units/Red/RedArcher"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedMage"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedChampion"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedMonarch")
            };

            List<ITexture2D> spriteTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadSpriteTextures)
            {
                spriteTextures.Add(new Texture2DWrapper(texture));
            }

            return spriteTextures;
        }

        public static List<ITexture2D> LoadLargePortraits(ContentManager content)
        {
            List<Texture2D> loadPortraitTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Images/Portraits/Large/Blue/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Large/Blue/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Large/Blue/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Large/Blue/Monarch"),

                content.Load<Texture2D>("Graphics/Images/Portraits/Large/Red/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Large/Red/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Large/Red/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Large/Red/Monarch")
            };

            List<ITexture2D> portraitTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadPortraitTextures)
            {
                portraitTextures.Add(new Texture2DWrapper(texture));
            }

            return portraitTextures;
        }

        public static List<ITexture2D> LoadMediumPortraits(ContentManager content)
        {
            List<Texture2D> loadPortraitTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Images/Portraits/Medium/Blue/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Medium/Blue/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Medium/Blue/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Medium/Blue/Monarch"),

                content.Load<Texture2D>("Graphics/Images/Portraits/Medium/Red/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Medium/Red/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Medium/Red/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Medium/Red/Monarch")
            };

            List<ITexture2D> portraitTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadPortraitTextures)
            {
                portraitTextures.Add(new Texture2DWrapper(texture));
            }

            return portraitTextures;
        }

        public static List<ITexture2D> LoadSmallPortraits(ContentManager content)
        {
            List<Texture2D> loadPortraitTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Images/Portraits/Small/Blue/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Small/Blue/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Small/Blue/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Small/Blue/Monarch"),

                content.Load<Texture2D>("Graphics/Images/Portraits/Small/Red/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Small/Red/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Small/Red/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Small/Red/Monarch")
            };

            List<ITexture2D> portraitTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadPortraitTextures)
            {
                portraitTextures.Add(new Texture2DWrapper(texture));
            }

            return portraitTextures;
        }

        public static ITexture2D LoadDiceAtlas(ContentManager content)
        {
            Texture2D diceTexture = content.Load<Texture2D>("Graphics/Images/Dice/AttackDiceAtlas");

            ITexture2D diceTextureWrapper = new Texture2DWrapper(diceTexture);

            return diceTextureWrapper;
        }

        public static ITexture2D LoadGameLogo(ContentManager content)
        {
            Texture2D backgroundTexture = content.Load<Texture2D>("Graphics/Images/Screens/SolStandard-LogoText_350");

            ITexture2D backgroundTextureWrapper = new Texture2DWrapper(backgroundTexture);

            return backgroundTextureWrapper;
        }

        public static ITexture2D LoadSolSpin(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Screens/SolSpin");

            ITexture2D textureWrapper = new Texture2DWrapper(loadTexture);

            return textureWrapper;
        }

        public static ITexture2D LoadTitleScreenBackground(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Screens/TitleBackground_BannerFlag");

            ITexture2D textureWrapper = new Texture2DWrapper(loadTexture);

            return textureWrapper;
        }

        public static List<ITexture2D> LoadMapPreviews(ContentManager content)
        {
            List<Texture2D> mapPreviewTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Grass_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Snow_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Desert_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Void_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Grass_02"),
            };

            List<ITexture2D> mapPreviewITextures = new List<ITexture2D>();
            foreach (Texture2D texture in mapPreviewTextures)
            {
                mapPreviewITextures.Add(new Texture2DWrapper(texture));
            }

            return mapPreviewITextures;
        }

        public static List<ITexture2D> LoadButtonIcons(ContentManager content)
        {
            List<Texture2D> buttonIconTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_A"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_B"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_X"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Y"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Dpad"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_LB"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_LT"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_RB"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_RT"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Left_Stick"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Right_Stick"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Windows"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Menu"),
            };

            List<ITexture2D> buttonIconITextures = new List<ITexture2D>();
            foreach (Texture2D texture in buttonIconTextures)
            {
                buttonIconITextures.Add(new Texture2DWrapper(texture));
            }

            return buttonIconITextures;
        }

        public static ISoundEffect LoadMenuMoveSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/menu-move-4");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMenuConfirmSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/menu-confirm-2");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMapCursorMoveSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/map-cursor-move");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMapUnitSelectSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/unit-selected-5");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMapUnitCancelSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/dice-roll");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadDiceRollSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/dice-roll");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMapUnitMoveSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/unit-footsteps");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadCombatStartSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/combat-start");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadCombatBlockSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/unit-block");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadCombatDamageSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/unit-damage");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadCombatDeathSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/unit-die-2");
            return new SoundEffectWrapper(effect, 0.8f);
        }

        public static ISoundEffect LoadDisableDiceSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/disable-dice");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static List<Song> LoadMusic(ContentManager content)
        {
            return new List<Song>
            {
                content.Load<Song>("Audio/Music/Game/MilitaryTheme"),
                content.Load<Song>("Audio/Music/Game/PlainsTheme"),
                content.Load<Song>("Audio/Music/Game/VoidTheme"),
                content.Load<Song>("Audio/Music/Game/DesertTheme"),
                content.Load<Song>("Audio/Music/Game/SnowyMountainTheme"),
                content.Load<Song>("Audio/Music/Game/IslandTheme"),
                content.Load<Song>("Audio/Music/Game/LavaTheme"),
                content.Load<Song>("Audio/Music/Game/MapSelectTheme"),
                content.Load<Song>("Audio/Music/Game/BossTheme"),
                content.Load<Song>("Audio/Music/Game/VictoryTheme")
            };
        }
    }
}