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
                content.Load<Texture2D>("Graphics/Map/Tiles/entities-32"),
                content.Load<Texture2D>("Graphics/Map/Tiles/overworld-32")
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
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedMonarch"),
                
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepSlime"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepTroll"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepOrc"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepMerchant"),
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
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Large/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Large/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Large/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Large/Monarch"),

                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Large/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Large/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Large/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Large/Monarch"),
                
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Large/Slime"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Large/Troll"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Large/Orc"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Large/Merchant"),
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
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Medium/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Medium/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Medium/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Medium/Monarch"),

                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Medium/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Medium/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Medium/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Medium/Monarch"),
                
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Medium/Slime"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Medium/Troll"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Medium/Orc"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Medium/Merchant"),
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
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Small/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Small/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Small/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Small/Monarch"),

                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Small/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Small/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Small/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Small/Monarch"),
                
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Small/Slime"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Small/Troll"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Small/Orc"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Small/Merchant"),
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

        public static ITexture2D LoadFireAtlas(ContentManager content)
        {
            Texture2D fireTexture = content.Load<Texture2D>("Graphics/Images/Icons/Misc/Fire");

            ITexture2D fireTextureWrapper = new Texture2DWrapper(fireTexture);

            return fireTextureWrapper;
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

        public static ITexture2D LoadGamepadKeyMap(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Screens/ButtonMap-Xbox");

            ITexture2D textureWrapper = new Texture2DWrapper(loadTexture);

            return textureWrapper;
        }

        public static ITexture2D LoadKeyboardKeyMap(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Screens/ButtonMap-PC");

            ITexture2D textureWrapper = new Texture2DWrapper(loadTexture);

            return textureWrapper;
        }

        public static List<ITexture2D> LoadMapPreviews(ContentManager content)
        {
            List<Texture2D> mapPreviewTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Grass_03"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Grass_04"),
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
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Dpad_Up"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Dpad_Down"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Dpad_Left"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Xbox/XboxOne_Dpad_Right"),
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


        public static List<ITexture2D> LoadSkillIcons(ContentManager content)
        {
            List<Texture2D> textures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/BasicAttack"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Blink"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Cover"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/DoubleTime"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Draw"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Inspire"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Shove"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Tackle"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Wait"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Harpoon"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Ignite"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Bulwark"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Atrophy"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Trap"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Bloodthirst"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Inferno"),
            };

            List<ITexture2D> skillTextures = new List<ITexture2D>();
            foreach (Texture2D texture in textures)
            {
                skillTextures.Add(new Texture2DWrapper(texture));
            }

            return skillTextures;
        }

        public static List<ITexture2D> LoadStatusIcons(ContentManager content)
        {
            List<Texture2D> textures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Images/Icons/StatusEffect/atkUp"),
                content.Load<Texture2D>("Graphics/Images/Icons/StatusEffect/atkRangeUp"),
                content.Load<Texture2D>("Graphics/Images/Icons/StatusEffect/defUp"),
                content.Load<Texture2D>("Graphics/Images/Icons/StatusEffect/hpUp"),
                content.Load<Texture2D>("Graphics/Images/Icons/StatusEffect/mvUp"),
                content.Load<Texture2D>("Graphics/Images/Icons/StatusEffect/spUp"),
            };

            List<ITexture2D> statusTextures = new List<ITexture2D>();
            foreach (Texture2D texture in textures)
            {
                statusTextures.Add(new Texture2DWrapper(texture));
            }

            return statusTextures;
        }

        public static ISoundEffect LoadMenuMoveSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_movement_ladder3b");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMenuConfirmSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_coin_cluster3");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMapCursorMoveSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_movement_ladder4a");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMapUnitSelectSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_coin_double1");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMapUnitCancelSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_weapon_singleshot4");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadDiceRollSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_weapon_singleshot4");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadMapUnitMoveSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/sfx_movement_footsteps1b");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadCombatStartSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/sfx_sounds_falling5");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadCombatBlockSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/sfx_sounds_impact11");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadCombatDamageSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/sfx_exp_shortest_hard6");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadCombatDeathSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/sfx_exp_short_hard1");
            return new SoundEffectWrapper(effect, 0.8f);
        }

        public static ISoundEffect LoadDisableDiceSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/sfx_wpn_punch2");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadWarningSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_sounds_impact12");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadSkillDrawSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/sfx_sounds_powerup3");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadSkillBlinkSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Character/sfx_movement_portal2");
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

        public static ITexture2D LoadGoldIcon(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Icons/Misc/gold");

            ITexture2D textureWrapper = new Texture2DWrapper(loadTexture);

            return textureWrapper;
        }

        public static ITexture2D LoadSpoilsIcon(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Icons/Misc/spoils");

            ITexture2D textureWrapper = new Texture2DWrapper(loadTexture);

            return textureWrapper;
        }

        public static ISoundEffect LoadCoinSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_coin_cluster7");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadDoorSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_exp_shortest_hard4");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadLockedSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_sounds_error5");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadUnlockSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_coin_double6");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadDropItemSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_exp_shortest_soft1");
            return new SoundEffectWrapper(effect, 0.5f);
        }

        public static ISoundEffect LoadErrorSFX(ContentManager content)
        {
            SoundEffect effect = content.Load<SoundEffect>("Audio/SFX/Interface/sfx_sounds_error3");
            return new SoundEffectWrapper(effect, 1f);
        }
    }
}