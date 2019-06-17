using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Load
{
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

        public static ISpriteFont LoadHeavyFont(ContentManager content)
        {
            return new SpriteFontWrapper(content.Load<SpriteFont>("Fonts/HeavyText"));
        }

        public static ITexture2D LoadWhitePixel(ContentManager content)
        {
            Texture2D spriteTextures = content.Load<Texture2D>("Graphics/WhitePixel");
            return new Texture2DWrapper(spriteTextures);
        }

        public static ITexture2D LoadWhiteGridOutline(ContentManager content)
        {
            return new Texture2DWrapper(content.Load<Texture2D>("Graphics/Map/Tiles/GridOutline"));
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
            return new Texture2DWrapper(content.Load<Texture2D>("Graphics/Map/Tiles/ActionTiles"));
        }

        public static ITexture2D LoadStatIcons(ContentManager content)
        {
            Texture2D statIconsTexture = content.Load<Texture2D>("Graphics/Images/Icons/StatIcons");
            return new Texture2DWrapper(statIconsTexture);
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

        public static ITexture2D LoadWindowTexture(ContentManager content)
        {
            Texture2D windowTexture = content.Load<Texture2D>("Graphics/WhitePixel");

            ITexture2D windowTextureWrapper = new Texture2DWrapper(windowTexture);

            return windowTextureWrapper;
        }

        public static List<ITexture2D> LoadUnitSpriteTextures(ContentManager content)
        {
            List<Texture2D> loadSpriteTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueArcher"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueMage"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueChampion"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueBard"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueLancer"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BluePugilist"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueDuelist"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueCleric"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueMarauder"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BluePaladin"),

                content.Load<Texture2D>("Graphics/Map/Units/Red/RedArcher"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedMage"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedChampion"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedBard"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedLancer"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedPugilist"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedDuelist"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedCleric"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedMarauder"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedPaladin"),

                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepSlime"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepTroll"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepOrc"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepMerchant"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepNecromancer"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepSkeleton"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepGoblin"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepRat"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepBat"),
                content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepSpider"),
            };

            List<ITexture2D> spriteTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadSpriteTextures)
            {
                spriteTextures.Add(new Texture2DWrapper(texture));
            }

            return spriteTextures;
        }


        public static List<ITexture2D> LoadSmallPortraits(ContentManager content)
        {
            List<Texture2D> loadPortraitTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Silhouette"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Bard"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Lancer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Pugilist"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Duelist"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Cleric"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Marauder"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Blue/Paladin"),

                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Silhouette"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Archer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Champion"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Mage"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Bard"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Lancer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Pugilist"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Duelist"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Cleric"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Marauder"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Red/Paladin"),

                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Silhouette"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Merchant"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Slime"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Troll"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Orc"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Necromancer"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Skeleton"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Goblin"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Rat"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Bat"),
                content.Load<Texture2D>("Graphics/Images/Portraits/Creep/Spider"),
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
            return new Texture2DWrapper(diceTexture);
        }

        public static ITexture2D LoadFireAtlas(ContentManager content)
        {
            Texture2D fireTexture = content.Load<Texture2D>("Graphics/Images/Icons/Misc/Fire");
            return new Texture2DWrapper(fireTexture);
        }

        public static ITexture2D LoadCommanderIcon(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Icons/Misc/CommanderCrown");
            return new Texture2DWrapper(loadTexture);
        }

        public static ITexture2D LoadGameLogo(ContentManager content)
        {
            Texture2D backgroundTexture =
                content.Load<Texture2D>("Graphics/Images/Screens/SolStandard-LogoText_350_v2");
            return new Texture2DWrapper(backgroundTexture);
        }

        public static ITexture2D LoadSolSpin(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Screens/SolSpin-White");
            return new Texture2DWrapper(loadTexture);
        }

        public static ITexture2D LoadTitleScreenBackground(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Screens/TitleBackground_BannerStripe");
            return new Texture2DWrapper(loadTexture);
        }

        public static ITexture2D LoadGamepadKeyMap(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Screens/ButtonMap-Xbox");
            return new Texture2DWrapper(loadTexture);
        }

        public static ITexture2D LoadKeyboardKeyMap(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Screens/ButtonMap-PC");
            return new Texture2DWrapper(loadTexture);
        }

        public static List<ITexture2D> LoadMapPreviews(ContentManager content)
        {
            List<Texture2D> mapPreviewTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Beach_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Village_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Chesslike_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Crossroads_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Dungeon_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Scotia_Hill_Redux_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_01_Arena"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_02_Dungeon"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Arena_Dungeon_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Arena_Grassland_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Arena_Tower_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Fortress_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Grassland_01"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Island_Coast"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Tavern_Inn"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Dimpimple_Beach"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Crossroads"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Jirai_Archipelago"),

                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Master_Control"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Verdant_Forest"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Raid_Dungeon"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Dusk_Temple"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Town_Market"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Collosseum_01"),

                content.Load<Texture2D>("Graphics/Map/MapPreviews/Solo_Alpha_Dungeon"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Escape_Prison"),
                content.Load<Texture2D>("Graphics/Map/MapPreviews/Draft_Hunt_Overworld_01"),
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

        public static List<ITexture2D> LoadKeyboardIcons(ContentManager content)
        {
            List<Texture2D> buttonIconTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Space"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Shift"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Q"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_E"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Tab"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_R"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Ctrl"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Alt"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_W"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_A"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_S"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_D"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Arrow_Up"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Arrow_Left"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Arrow_Down"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Arrow_Right"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Enter"),
                content.Load<Texture2D>("Graphics/HUD/Buttons/Keyboard/Keyboard_Black_Esc"),
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
                //Skill Icons
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/BasicAttack"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Blink"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/DoubleTime"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Draw"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Inspire"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Shove"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Tackle"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Wait"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Ignite"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Bulwark"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Atrophy"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Trap"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Bloodthirst"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Inferno"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/PoisonTip"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Assassinate"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Charge"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Intervention"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Meditate"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Uppercut"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/AtkBuff"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Challenge"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/PhaseStrike"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Focus"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Cleanse"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Rage"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Guillotine"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Brace"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Rampart"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Intervention"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Stun"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Recover"),
                
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/BetwixtPlate"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/FlowStrike"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Frostbite"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/LeapStrike"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Suplex"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Terraform"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Venom"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Grapple"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Fortify"),
                //AI Routine Icons
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Prey"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Kingslayer"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Defender"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/TriggerHappy"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/TreasureHunter"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Glutton"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Summon"),
                content.Load<Texture2D>("Graphics/Images/Icons/Skill/Wander"),
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
                content.Load<Texture2D>("Graphics/Images/Icons/StatusEffect/moraleBroken"),
                content.Load<Texture2D>("Graphics/Images/Icons/Misc/clock"),
                content.Load<Texture2D>("Graphics/Images/Icons/Misc/hand"),
                content.Load<Texture2D>("Graphics/Images/Icons/Misc/durability"),
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
            return new Texture2DWrapper(loadTexture);
        }

        public static ITexture2D LoadSpoilsIcon(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Icons/Misc/spoils");
            return new Texture2DWrapper(loadTexture);
        }

        public static ITexture2D LoadObjectiveIcons(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Icons/ObjectiveIcons16");
            return new Texture2DWrapper(loadTexture);
        }

        public static ITexture2D LoadTeamIcons(ContentManager content)
        {
            Texture2D loadTexture = content.Load<Texture2D>("Graphics/Images/Icons/SolLunaTerra");
            return new Texture2DWrapper(loadTexture);
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