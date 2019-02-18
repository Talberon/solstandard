using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum ButtonIcon
    {
        A,
        B,
        X,
        Y,
        Dpad,
        Lb,
        Lt,
        Rb,
        Rt,
        LeftStick,
        RightStick,
        Windows,
        Menu,
        DpadUp,
        DpadDown,
        DpadLeft,
        DpadRight
    }

    public static class ButtonIconProvider
    {
        private static Dictionary<ButtonIcon, ITexture2D> _buttonDictionary;

        public static SpriteAtlas GetButton(ButtonIcon icon, Vector2 iconSize)
        {
            return new SpriteAtlas(_buttonDictionary[icon],
                new Vector2(_buttonDictionary[icon].Width, _buttonDictionary[icon].Height), iconSize);
        }

        public static void LoadButtons(List<ITexture2D> buttonTextures)
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
            ITexture2D textureRightStick = buttonTextures.Find(texture => texture.Name.EndsWith("_Right_Stick"));
            ITexture2D textureLeftStick = buttonTextures.Find(texture => texture.Name.EndsWith("_Left_Stick"));
            ITexture2D textureWindows = buttonTextures.Find(texture => texture.Name.EndsWith("_Windows"));
            ITexture2D textureMenu = buttonTextures.Find(texture => texture.Name.EndsWith("_Menu"));

            _buttonDictionary = new Dictionary<ButtonIcon, ITexture2D>
            {
                {ButtonIcon.A, textureA},
                {ButtonIcon.B, textureB},
                {ButtonIcon.X, textureX},
                {ButtonIcon.Y, textureY},
                {ButtonIcon.Dpad, textureDpad},
                {ButtonIcon.DpadUp, textureDpadUp},
                {ButtonIcon.DpadDown, textureDpadDown},
                {ButtonIcon.DpadLeft, textureDpadLeft},
                {ButtonIcon.DpadRight, textureDpadRight},
                {ButtonIcon.Lb, textureLb},
                {ButtonIcon.Lt, textureLt},
                {ButtonIcon.Rb, textureRb},
                {ButtonIcon.Rt, textureRt},
                {ButtonIcon.LeftStick, textureRightStick},
                {ButtonIcon.RightStick, textureLeftStick},
                {ButtonIcon.Windows, textureWindows},
                {ButtonIcon.Menu, textureMenu}
            };
        }
    }
}