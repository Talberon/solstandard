using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.NeoGFX.Graphics;
using SolStandard.NeoUtility.Monogame.Interfaces;

namespace SolStandard.NeoUtility.Monogame.Assets
{
    public enum GamepadIcon
    {
        A,
        B,
        X,
        Y,

        Dpad,
        DpadUp,
        DpadDown,
        DpadLeft,
        DpadRight,

        LeftStick,
        LeftStickUp,
        LeftStickDown,
        LeftStickLeft,
        LeftStickRight,

        Windows,
        Menu,

        RightStick,
        RightStickUp,
        RightStickDown,
        RightStickLeft,
        RightStickRight,

        Lb,
        Lt,
        Rb,
        Rt,
    }

    public static class GamepadIconProvider
    {
        // ReSharper disable once NotNullMemberIsNotInitialized
        private static Dictionary<GamepadIcon, ITexture2D> _buttonDictionary;

        public static SpriteAtlas GetButton(GamepadIcon icon, Vector2 iconSize)
        {
            return _buttonDictionary[icon].ToSingleImageSprite(iconSize);
        }

        public static void LoadIcons(List<ITexture2D> buttonTextures)
        {
            ITexture2D textureA = buttonTextures.Find(texture => texture.Name.EndsWith("_A"));
            ITexture2D textureB = buttonTextures.Find(texture => texture.Name.EndsWith("_B"));
            ITexture2D textureX = buttonTextures.Find(texture => texture.Name.EndsWith("_X"));
            ITexture2D textureY = buttonTextures.Find(texture => texture.Name.EndsWith("_Y"));
            ITexture2D textureDpad = buttonTextures.Find(texture => texture.Name.EndsWith("_Dpad"));
            ITexture2D textureDpadUp = buttonTextures.Find(texture => texture.Name.EndsWith("_Dpad_Up"));
            ITexture2D textureDpadDown = buttonTextures.Find(texture => texture.Name.EndsWith("_Dpad_Down"));
            ITexture2D textureDpadLeft = buttonTextures.Find(texture => texture.Name.EndsWith("_Dpad_Left"));
            ITexture2D textureDpadRight = buttonTextures.Find(texture => texture.Name.EndsWith("_Dpad_Right"));
            ITexture2D textureRb = buttonTextures.Find(texture => texture.Name.EndsWith("_RB"));
            ITexture2D textureRt = buttonTextures.Find(texture => texture.Name.EndsWith("_RT"));
            ITexture2D textureLb = buttonTextures.Find(texture => texture.Name.EndsWith("_LB"));
            ITexture2D textureLt = buttonTextures.Find(texture => texture.Name.EndsWith("_LT"));
            ITexture2D textureRs = buttonTextures.Find(texture => texture.Name.EndsWith("_Right_Stick"));
            ITexture2D textureRsUp = buttonTextures.Find(texture => texture.Name.EndsWith("_Right_Stick_Up"));
            ITexture2D textureRsDown = buttonTextures.Find(texture => texture.Name.EndsWith("_Right_Stick_Down"));
            ITexture2D textureRsLeft = buttonTextures.Find(texture => texture.Name.EndsWith("_Right_Stick_Left"));
            ITexture2D textureRsRight = buttonTextures.Find(texture => texture.Name.EndsWith("_Right_Stick_Right"));
            ITexture2D textureLs = buttonTextures.Find(texture => texture.Name.EndsWith("_Left_Stick"));
            ITexture2D textureLsUp = buttonTextures.Find(texture => texture.Name.EndsWith("_Left_Stick_Up"));
            ITexture2D textureLsDown = buttonTextures.Find(texture => texture.Name.EndsWith("_Left_Stick_Down"));
            ITexture2D textureLsLeft = buttonTextures.Find(texture => texture.Name.EndsWith("_Left_Stick_Left"));
            ITexture2D textureLsRight = buttonTextures.Find(texture => texture.Name.EndsWith("_Left_Stick_Right"));
            ITexture2D textureWindows = buttonTextures.Find(texture => texture.Name.EndsWith("_Windows"));
            ITexture2D textureMenu = buttonTextures.Find(texture => texture.Name.EndsWith("_Menu"));

            _buttonDictionary = new Dictionary<GamepadIcon, ITexture2D>
            {
                {GamepadIcon.A, textureA},
                {GamepadIcon.B, textureB},
                {GamepadIcon.X, textureX},
                {GamepadIcon.Y, textureY},
                {GamepadIcon.Dpad, textureDpad},
                {GamepadIcon.DpadUp, textureDpadUp},
                {GamepadIcon.DpadDown, textureDpadDown},
                {GamepadIcon.DpadLeft, textureDpadLeft},
                {GamepadIcon.DpadRight, textureDpadRight},
                {GamepadIcon.Lb, textureLb},
                {GamepadIcon.Lt, textureLt},
                {GamepadIcon.Rb, textureRb},
                {GamepadIcon.Rt, textureRt},
                {GamepadIcon.LeftStick, textureLs},
                {GamepadIcon.LeftStickUp, textureLsUp},
                {GamepadIcon.LeftStickDown, textureLsDown},
                {GamepadIcon.LeftStickLeft, textureLsLeft},
                {GamepadIcon.LeftStickRight, textureLsRight},
                {GamepadIcon.RightStick, textureRs},
                {GamepadIcon.RightStickUp, textureRsUp},
                {GamepadIcon.RightStickDown, textureRsDown},
                {GamepadIcon.RightStickLeft, textureRsLeft},
                {GamepadIcon.RightStickRight, textureRsRight},
                {GamepadIcon.Windows, textureWindows},
                {GamepadIcon.Menu, textureMenu}
            };
        }
    }
}