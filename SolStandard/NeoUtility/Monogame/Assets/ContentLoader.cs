using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using Steelbreakers.Utility.Monogame.Interfaces;

namespace Steelbreakers.Utility.Monogame.Assets
{
    public static class ContentLoader
    {
        // ReSharper disable once NotNullMemberIsNotInitialized
        private static ContentManager _contentManager;

        public static void RegisterContentManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public static ITexture2D LoadTileImage(string imageName)
        {
            var texture = _contentManager.Load<Texture2D>($"Graphics/Tiles/{imageName}");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadRoundedWindowTexture()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Icons/HUD/window-rounded");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadSolIconHiRes()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/HighResolution/sol-icon-hires-black");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadSpotlightTexture()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Objects/spotlight");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadSharpWindowTexture()
        {
            var texture = _contentManager.Load<Texture2D>($"Graphics/Icons/HUD/window-sharp");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadWhitePixel()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/white-pixel");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadDustParticle()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Particles/dust");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadSkullParticle()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Particles/skull");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadShadowParticle()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Particles/shadow");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadShieldParticle()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Particles/shield");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadHeartParticle()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Particles/heart");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadSmallShadowParticle()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Particles/shadow-small");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadDebrisParticle()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Particles/debris");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadSparkleParticle()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Particles/sparkle");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadTitleScreenBackground()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/HighResolution/title-screen");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadRandomBackground()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/HighResolution/random-background");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadWeaponBackground()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/HighResolution/weapon-background");
            return new Texture2DWrapper(texture);
        }

        public static ITexture2D LoadRandomIcon()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/HighResolution/random");
            return new Texture2DWrapper(texture);
        }

        public static ISpriteFont LoadTitleFont()
        {
            var font = _contentManager.Load<SpriteFont>("Fonts/TitleFont");
            return new SpriteFontWrapper(font);
        }

        public static ISpriteFont LoadWindowFont()
        {
            var font = _contentManager.Load<SpriteFont>("Fonts/BasicPixelFont");
            return new SpriteFontWrapper(font);
        }

        public static ISpriteFont LoadTinyFont()
        {
            var font = _contentManager.Load<SpriteFont>("Fonts/TinyPixelFont");
            return new SpriteFontWrapper(font);
        }

        public static AnimatedSprite LoadRedPlayerSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Player/neo-hero-red");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Player/neo-hero-red-animation");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadBluePlayerSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Player/neo-hero-blue");
            var animation =
                _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Player/neo-hero-blue-animation");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadGreenPlayerSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Player/neo-hero-green");
            var animation =
                _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Player/neo-hero-green-animation");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadYellowPlayerSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Player/neo-hero-yellow");
            var animation =
                _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Player/neo-hero-yellow-animation");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadRedPlayerPortrait()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Player/Portraits/portrait-male-hero-red");
            var animation = _contentManager.Load<AnimationDefinition>(
                "Graphics/Sprites/Player/Portraits/portrait-male-hero-red-data"
            );
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadBluePlayerPortrait()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Player/Portraits/portrait-male-hero-blue");
            var animation = _contentManager.Load<AnimationDefinition>(
                "Graphics/Sprites/Player/Portraits/portrait-male-hero-blue-data"
            );
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadGreenPlayerPortrait()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Player/Portraits/portrait-male-hero-green");
            var animation = _contentManager.Load<AnimationDefinition>(
                "Graphics/Sprites/Player/Portraits/portrait-male-hero-green-data"
            );
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadYellowPlayerPortrait()
        {
            var texture =
                _contentManager.Load<Texture2D>("Graphics/Sprites/Player/Portraits/portrait-male-hero-yellow");
            var animation = _contentManager.Load<AnimationDefinition>(
                "Graphics/Sprites/Player/Portraits/portrait-male-hero-yellow-data"
            );
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadSlimeSprite(string colorName)
        {
            var texture = _contentManager.Load<Texture2D>($"Graphics/Sprites/Creeps/slime-{colorName}");
            var animation =
                _contentManager.Load<AnimationDefinition>($"Graphics/Sprites/Creeps/slime-{colorName}-animation");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadOrcSprite(string colorName)
        {
            var texture = _contentManager.Load<Texture2D>($"Graphics/Sprites/Creeps/orc-{colorName}");
            var animation =
                _contentManager.Load<AnimationDefinition>($"Graphics/Sprites/Creeps/orc-{colorName}-animation");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadArrowSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Objects/arrow");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Objects/arrow-data");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadJavelinSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Objects/javelin");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Objects/javelin-data");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadItemsSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Objects/items");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Objects/items-data");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadHeartPieceSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Objects/heart-piece");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Objects/heart-piece-data");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadArmorPieceSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Objects/armor-piece");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Objects/armor-piece-data");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadCombatHudSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Combat/combat-hud");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/Icons/HUD/Combat/combat-hud-data");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadStatefulObjectsSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Sprites/Objects/stateful-objects");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/Sprites/Objects/stateful-objects-data");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadObjectiveIconsSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Combat/objectives");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/Icons/HUD/Combat/objectives-data");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadLogoSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/HighResolution/steelbreakers-logo-full");
            var animation =
                _contentManager.Load<AnimationDefinition>("Graphics/HighResolution/steelbreakers-logo-full-data");
            return new AnimatedSprite(texture, animation);
        }

        public static AnimatedSprite LoadDeveloperSplashLogoSprite()
        {
            var texture = _contentManager.Load<Texture2D>("Graphics/HighResolution/talberon-games");
            var animation = _contentManager.Load<AnimationDefinition>("Graphics/HighResolution/talberon-games-data");
            return new AnimatedSprite(texture, animation);
        }

        public static ISoundEffect LoadSFX(string sfxName, float volumeZeroToOne, float variance = 0f)
        {
            var sfx = _contentManager.Load<SoundEffect>($"Audio/SFX/{sfxName}");
            return new SoundEffectWrapper(sfx, volumeZeroToOne, variance);
        }

        public static IPlayableAudio LoadBGM(string bgmName)
        {
            var bgm = _contentManager.Load<SoundEffect>($"Audio/Music/{bgmName}");
            return new SoundEffectWrapper(bgm, 1f, 0f);
        }

        public static List<ITexture2D> LoadKeyboardIcons()
        {
            var buttonIconTextures = new List<Texture2D>
            {
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Space"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Shift"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Q"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_E"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Tab"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_R"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Ctrl"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Alt"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_W"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_A"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_S"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_D"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Arrow_Up"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Arrow_Left"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Arrow_Down"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Arrow_Right"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Enter"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Esc"),

                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Apostrophe"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Backslash"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Backspace"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Bracket_Left"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Bracket_Right"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Comma"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Equals"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Forwardslash"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Minus"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Period"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Semicolon"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Tilde"),

                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_T"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Y"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_U"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_I"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_O"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_P"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_F"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_G"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_H"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_J"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_K"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_L"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_Z"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_X"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_C"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_V"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_B"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_N"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_M"),

                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_1"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_2"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_3"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_4"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_5"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_6"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_7"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_8"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_9"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Keyboard/Keyboard_Black_0"),
            };

            return buttonIconTextures.Select(texture => new Texture2DWrapper(texture)).Cast<ITexture2D>().ToList();
        }

        public static List<ITexture2D> LoadGamepadIcons()
        {
            var buttonIconTextures = new List<Texture2D>
            {
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_A"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_B"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_X"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Y"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Dpad"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Dpad_Up"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Dpad_Down"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Dpad_Left"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Dpad_Right"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_LB"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_LT"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_RB"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_RT"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Left_Stick"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Left_Stick_Up"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Left_Stick_Down"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Left_Stick_Left"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Left_Stick_Right"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Right_Stick"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Right_Stick_Up"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Right_Stick_Down"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Right_Stick_Left"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Right_Stick_Right"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Windows"),
                _contentManager.Load<Texture2D>("Graphics/Icons/HUD/Xbox/XboxOne_Menu")
            };

            return buttonIconTextures.Select(texture => new Texture2DWrapper(texture)).Cast<ITexture2D>().ToList();
        }

        public static ITexture2D LoadMapPreviewImage(string mapName)
        {
            var texture = _contentManager.Load<Texture2D>($"Tiled/Maps/{mapName}/{mapName}");
            return new Texture2DWrapper(texture);
        }
    }
}