using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum KeyboardIcon
    {
        Space,
        LeftShift,
        Q,
        E,

        W,
        A,
        S,
        D,

        Enter,
        Escape,

        Up,
        Left,
        Down,
        Right,

        Tab,
        R,
        LeftCtrl,
        LeftAlt
    }

    public static class KeyboardIconProvider
    {
        private static Dictionary<KeyboardIcon, ITexture2D> _buttonDictionary;

        public static SpriteAtlas GetKeyboardIcon(KeyboardIcon icon, Vector2 iconSize)
        {
            return new SpriteAtlas(
                _buttonDictionary[icon],
                new Vector2(_buttonDictionary[icon].Width, _buttonDictionary[icon].Height),
                iconSize
            );
        }

        public static void LoadIcons(List<ITexture2D> keyboardTextures)
        {
            ITexture2D textureSpace = keyboardTextures.Find(texture => texture.Name.EndsWith("_Space"));
            ITexture2D textureLeftShift = keyboardTextures.Find(texture => texture.Name.EndsWith("_Shift"));
            ITexture2D textureQ = keyboardTextures.Find(texture => texture.Name.EndsWith("_Q"));
            ITexture2D textureE = keyboardTextures.Find(texture => texture.Name.EndsWith("_E"));
            ITexture2D textureTab = keyboardTextures.Find(texture => texture.Name.EndsWith("_Tab"));
            ITexture2D textureR = keyboardTextures.Find(texture => texture.Name.EndsWith("_R"));
            ITexture2D textureLeftCtrl = keyboardTextures.Find(texture => texture.Name.EndsWith("_Ctrl"));
            ITexture2D textureLeftAlt = keyboardTextures.Find(texture => texture.Name.EndsWith("_Alt"));
            ITexture2D textureW = keyboardTextures.Find(texture => texture.Name.EndsWith("_W"));
            ITexture2D textureA = keyboardTextures.Find(texture => texture.Name.EndsWith("_A"));
            ITexture2D textureS = keyboardTextures.Find(texture => texture.Name.EndsWith("_S"));
            ITexture2D textureD = keyboardTextures.Find(texture => texture.Name.EndsWith("_D"));
            ITexture2D textureUp = keyboardTextures.Find(texture => texture.Name.EndsWith("_Arrow_Up"));
            ITexture2D textureLeft = keyboardTextures.Find(texture => texture.Name.EndsWith("_Arrow_Left"));
            ITexture2D textureDown = keyboardTextures.Find(texture => texture.Name.EndsWith("_Arrow_Down"));
            ITexture2D textureRight = keyboardTextures.Find(texture => texture.Name.EndsWith("_Arrow_Right"));
            ITexture2D textureEnter = keyboardTextures.Find(texture => texture.Name.EndsWith("_Enter"));
            ITexture2D textureEscape = keyboardTextures.Find(texture => texture.Name.EndsWith("_Esc"));

            _buttonDictionary = new Dictionary<KeyboardIcon, ITexture2D>
            {
                {KeyboardIcon.Space, textureSpace},
                {KeyboardIcon.LeftShift, textureLeftShift},
                {KeyboardIcon.Q, textureQ},
                {KeyboardIcon.E, textureE},
                {KeyboardIcon.Tab, textureTab},
                {KeyboardIcon.R, textureR},
                {KeyboardIcon.LeftCtrl, textureLeftCtrl},
                {KeyboardIcon.LeftAlt, textureLeftAlt},
                {KeyboardIcon.W, textureW},
                {KeyboardIcon.A, textureA},
                {KeyboardIcon.S, textureS},
                {KeyboardIcon.D, textureD},
                {KeyboardIcon.Up, textureUp},
                {KeyboardIcon.Left, textureLeft},
                {KeyboardIcon.Down, textureDown},
                {KeyboardIcon.Right, textureRight},
                {KeyboardIcon.Enter, textureEnter},
                {KeyboardIcon.Escape, textureEscape}
            };
        }
    }
}