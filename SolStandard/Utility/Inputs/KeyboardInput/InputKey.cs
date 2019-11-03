using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Inputs.KeyboardInput
{
    [Serializable]
    public class InputKey : GameControl
    {
        public static readonly IReadOnlyDictionary<Keys, KeyboardIcon> KeyIcons = new Dictionary<Keys, KeyboardIcon>
        {
            {Keys.A, KeyboardIcon.A},
            {Keys.B, KeyboardIcon.B},
            {Keys.C, KeyboardIcon.C},
            {Keys.D, KeyboardIcon.D},
            {Keys.E, KeyboardIcon.E},
            {Keys.F, KeyboardIcon.F},
            {Keys.G, KeyboardIcon.G},
            {Keys.H, KeyboardIcon.H},
            {Keys.I, KeyboardIcon.I},
            {Keys.J, KeyboardIcon.J},
            {Keys.K, KeyboardIcon.K},
            {Keys.L, KeyboardIcon.L},
            {Keys.M, KeyboardIcon.M},
            {Keys.N, KeyboardIcon.N},
            {Keys.O, KeyboardIcon.O},
            {Keys.P, KeyboardIcon.P},
            {Keys.Q, KeyboardIcon.Q},
            {Keys.R, KeyboardIcon.R},
            {Keys.S, KeyboardIcon.S},
            {Keys.T, KeyboardIcon.T},
            {Keys.U, KeyboardIcon.U},
            {Keys.V, KeyboardIcon.V},
            {Keys.W, KeyboardIcon.W},
            {Keys.X, KeyboardIcon.X},
            {Keys.Y, KeyboardIcon.Y},
            {Keys.Z, KeyboardIcon.Z},

            {Keys.D0, KeyboardIcon.Zero},
            {Keys.D1, KeyboardIcon.One},
            {Keys.D2, KeyboardIcon.Two},
            {Keys.D3, KeyboardIcon.Three},
            {Keys.D4, KeyboardIcon.Four},
            {Keys.D5, KeyboardIcon.Five},
            {Keys.D6, KeyboardIcon.Six},
            {Keys.D7, KeyboardIcon.Seven},
            {Keys.D8, KeyboardIcon.Eight},
            {Keys.D9, KeyboardIcon.Nine},

            {Keys.OemQuotes, KeyboardIcon.Apostrophe},
            {Keys.OemPipe, KeyboardIcon.Backslash},
            {Keys.Back, KeyboardIcon.Backspace},
            {Keys.OemOpenBrackets, KeyboardIcon.BracketLeft},
            {Keys.OemCloseBrackets, KeyboardIcon.BracketRight},
            {Keys.OemComma, KeyboardIcon.Comma},
            {Keys.OemPlus, KeyboardIcon.Equals},
            {Keys.OemQuestion, KeyboardIcon.Forwardslash},
            {Keys.OemMinus, KeyboardIcon.Minus},
            {Keys.OemPeriod, KeyboardIcon.Period},
            {Keys.OemSemicolon, KeyboardIcon.Semicolon},
            {Keys.OemTilde, KeyboardIcon.Tilde},

            {Keys.LeftShift, KeyboardIcon.LeftShift},
            {Keys.LeftAlt, KeyboardIcon.LeftAlt},
            {Keys.LeftControl, KeyboardIcon.LeftCtrl},
            {Keys.RightShift, KeyboardIcon.RightShift},
            {Keys.RightAlt, KeyboardIcon.RightAlt},
            {Keys.RightControl, KeyboardIcon.RightCtrl},

            {Keys.Space, KeyboardIcon.Space},
            {Keys.Enter, KeyboardIcon.Enter},
            {Keys.Escape, KeyboardIcon.Escape},
            {Keys.Tab, KeyboardIcon.Tab},

            {Keys.Up, KeyboardIcon.Up},
            {Keys.Down, KeyboardIcon.Down},
            {Keys.Left, KeyboardIcon.Left},
            {Keys.Right, KeyboardIcon.Right},
        };

        private readonly Keys key;

        public InputKey(Keys key)
        {
            this.key = key;
        }

        public override bool Pressed => Keyboard.GetState().IsKeyDown(key);

        public override IRenderable GetInputIcon(int iconSize)
        {
            return KeyboardIconProvider.GetKeyboardIcon(
                (!KeyIcons.ContainsKey(key))
                    ? KeyboardIcon.Space
                    : KeyIcons[key],
                new Vector2(iconSize)
            );
        }

        public override bool Equals(object obj)
        {
            return (obj is InputKey inputKey && inputKey.key == key);
        }

        public override int GetHashCode()
        {
            return (int) key;
        }
    }
}