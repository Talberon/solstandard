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
        LeftAlt,

        T,
        Y,
        U,
        I,
        O,
        P,
        F,
        G,
        H,
        J,
        K,
        L,
        Z,
        X,
        C,
        V,
        B,
        N,
        M,

        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Zero,

        RightShift,
        RightAlt,
        RightCtrl,

        Apostrophe,
        Backslash,
        Backspace,
        BracketLeft,
        BracketRight,
        Comma,
        Equals,
        Forwardslash,
        Minus,
        Period,
        Semicolon,
        Tilde,
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
            ITexture2D textureShift = keyboardTextures.Find(texture => texture.Name.EndsWith("_Shift"));
            ITexture2D textureQ = keyboardTextures.Find(texture => texture.Name.EndsWith("_Q"));
            ITexture2D textureE = keyboardTextures.Find(texture => texture.Name.EndsWith("_E"));
            ITexture2D textureTab = keyboardTextures.Find(texture => texture.Name.EndsWith("_Tab"));
            ITexture2D textureR = keyboardTextures.Find(texture => texture.Name.EndsWith("_R"));
            ITexture2D textureCtrl = keyboardTextures.Find(texture => texture.Name.EndsWith("_Ctrl"));
            ITexture2D textureAlt = keyboardTextures.Find(texture => texture.Name.EndsWith("_Alt"));
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

            ITexture2D textureApostrophe = keyboardTextures.Find(texture => texture.Name.EndsWith("_Apostrophe"));
            ITexture2D textureBackslash = keyboardTextures.Find(texture => texture.Name.EndsWith("_Backslash"));
            ITexture2D textureBackspace = keyboardTextures.Find(texture => texture.Name.EndsWith("_Backspace"));
            ITexture2D textureBracketLeft = keyboardTextures.Find(texture => texture.Name.EndsWith("_Bracket_Left"));
            ITexture2D textureBracketRight = keyboardTextures.Find(texture => texture.Name.EndsWith("_Bracket_Right"));
            ITexture2D textureComma = keyboardTextures.Find(texture => texture.Name.EndsWith("_Comma"));
            ITexture2D textureEquals = keyboardTextures.Find(texture => texture.Name.EndsWith("_Equals"));
            ITexture2D textureForwardslash = keyboardTextures.Find(texture => texture.Name.EndsWith("_Forwardslash"));
            ITexture2D textureMinus = keyboardTextures.Find(texture => texture.Name.EndsWith("_Minus"));
            ITexture2D texturePeriod = keyboardTextures.Find(texture => texture.Name.EndsWith("_Period"));
            ITexture2D textureSemicolon = keyboardTextures.Find(texture => texture.Name.EndsWith("_Semicolon"));
            ITexture2D textureTilde = keyboardTextures.Find(texture => texture.Name.EndsWith("_Tilde"));

            ITexture2D textureT = keyboardTextures.Find(texture => texture.Name.EndsWith("_T"));
            ITexture2D textureY = keyboardTextures.Find(texture => texture.Name.EndsWith("_Y"));
            ITexture2D textureU = keyboardTextures.Find(texture => texture.Name.EndsWith("_U"));
            ITexture2D textureI = keyboardTextures.Find(texture => texture.Name.EndsWith("_I"));
            ITexture2D textureO = keyboardTextures.Find(texture => texture.Name.EndsWith("_O"));
            ITexture2D textureP = keyboardTextures.Find(texture => texture.Name.EndsWith("_P"));
            ITexture2D textureF = keyboardTextures.Find(texture => texture.Name.EndsWith("_F"));
            ITexture2D textureG = keyboardTextures.Find(texture => texture.Name.EndsWith("_G"));
            ITexture2D textureH = keyboardTextures.Find(texture => texture.Name.EndsWith("_H"));
            ITexture2D textureJ = keyboardTextures.Find(texture => texture.Name.EndsWith("_J"));
            ITexture2D textureK = keyboardTextures.Find(texture => texture.Name.EndsWith("_K"));
            ITexture2D textureL = keyboardTextures.Find(texture => texture.Name.EndsWith("_L"));
            ITexture2D textureZ = keyboardTextures.Find(texture => texture.Name.EndsWith("_Z"));
            ITexture2D textureX = keyboardTextures.Find(texture => texture.Name.EndsWith("_X"));
            ITexture2D textureC = keyboardTextures.Find(texture => texture.Name.EndsWith("_C"));
            ITexture2D textureV = keyboardTextures.Find(texture => texture.Name.EndsWith("_V"));
            ITexture2D textureB = keyboardTextures.Find(texture => texture.Name.EndsWith("_B"));
            ITexture2D textureN = keyboardTextures.Find(texture => texture.Name.EndsWith("_N"));
            ITexture2D textureM = keyboardTextures.Find(texture => texture.Name.EndsWith("_M"));

            ITexture2D textureOne = keyboardTextures.Find(texture => texture.Name.EndsWith("_1"));
            ITexture2D textureTwo = keyboardTextures.Find(texture => texture.Name.EndsWith("_2"));
            ITexture2D textureThree = keyboardTextures.Find(texture => texture.Name.EndsWith("_3"));
            ITexture2D textureFour = keyboardTextures.Find(texture => texture.Name.EndsWith("_4"));
            ITexture2D textureFive = keyboardTextures.Find(texture => texture.Name.EndsWith("_5"));
            ITexture2D textureSix = keyboardTextures.Find(texture => texture.Name.EndsWith("_6"));
            ITexture2D textureSeven = keyboardTextures.Find(texture => texture.Name.EndsWith("_7"));
            ITexture2D textureEight = keyboardTextures.Find(texture => texture.Name.EndsWith("_8"));
            ITexture2D textureNine = keyboardTextures.Find(texture => texture.Name.EndsWith("_9"));
            ITexture2D textureZero = keyboardTextures.Find(texture => texture.Name.EndsWith("_0"));

            _buttonDictionary = new Dictionary<KeyboardIcon, ITexture2D>
            {
                {KeyboardIcon.Space, textureSpace},
                {KeyboardIcon.LeftShift, textureShift},
                {KeyboardIcon.Q, textureQ},
                {KeyboardIcon.E, textureE},
                {KeyboardIcon.Tab, textureTab},
                {KeyboardIcon.R, textureR},
                {KeyboardIcon.LeftCtrl, textureCtrl},
                {KeyboardIcon.LeftAlt, textureAlt},
                {KeyboardIcon.W, textureW},
                {KeyboardIcon.A, textureA},
                {KeyboardIcon.S, textureS},
                {KeyboardIcon.D, textureD},
                {KeyboardIcon.Up, textureUp},
                {KeyboardIcon.Left, textureLeft},
                {KeyboardIcon.Down, textureDown},
                {KeyboardIcon.Right, textureRight},
                {KeyboardIcon.Enter, textureEnter},
                {KeyboardIcon.Escape, textureEscape},

                {KeyboardIcon.Apostrophe, textureApostrophe},
                {KeyboardIcon.Backslash, textureBackslash},
                {KeyboardIcon.Backspace, textureBackspace},
                {KeyboardIcon.BracketLeft, textureBracketLeft},
                {KeyboardIcon.BracketRight, textureBracketRight},
                {KeyboardIcon.Comma, textureComma},
                {KeyboardIcon.Equals, textureEquals},
                {KeyboardIcon.Forwardslash, textureForwardslash},
                {KeyboardIcon.Minus, textureMinus},
                {KeyboardIcon.Period, texturePeriod},
                {KeyboardIcon.Semicolon, textureSemicolon},
                {KeyboardIcon.Tilde, textureTilde},

                {KeyboardIcon.T, textureT},
                {KeyboardIcon.Y, textureY},
                {KeyboardIcon.U, textureU},
                {KeyboardIcon.I, textureI},
                {KeyboardIcon.O, textureO},
                {KeyboardIcon.P, textureP},
                {KeyboardIcon.F, textureF},
                {KeyboardIcon.G, textureG},
                {KeyboardIcon.H, textureH},
                {KeyboardIcon.J, textureJ},
                {KeyboardIcon.K, textureK},
                {KeyboardIcon.L, textureL},
                {KeyboardIcon.Z, textureZ},
                {KeyboardIcon.X, textureX},
                {KeyboardIcon.C, textureC},
                {KeyboardIcon.V, textureV},
                {KeyboardIcon.B, textureB},
                {KeyboardIcon.N, textureN},
                {KeyboardIcon.M, textureM},


                {KeyboardIcon.One, textureOne},
                {KeyboardIcon.Two, textureTwo},
                {KeyboardIcon.Three, textureThree},
                {KeyboardIcon.Four, textureFour},
                {KeyboardIcon.Five, textureFive},
                {KeyboardIcon.Six, textureSix},
                {KeyboardIcon.Seven, textureSeven},
                {KeyboardIcon.Eight, textureEight},
                {KeyboardIcon.Nine, textureNine},
                {KeyboardIcon.Zero, textureZero},

                {KeyboardIcon.RightShift, textureShift},
                {KeyboardIcon.RightAlt, textureAlt},
                {KeyboardIcon.RightCtrl, textureCtrl},
            };
        }
    }
}