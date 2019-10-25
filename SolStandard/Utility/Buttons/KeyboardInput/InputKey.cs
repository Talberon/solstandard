using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.KeyboardInput
{
    public class InputKey : GameControl
    {
        private static readonly Dictionary<Keys, KeyboardIcon> KeyIcons = new Dictionary<Keys, KeyboardIcon>
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

            {Keys.LeftShift, KeyboardIcon.LeftShift},
            {Keys.LeftAlt, KeyboardIcon.LeftAlt},
            {Keys.LeftControl, KeyboardIcon.LeftCtrl},
            {Keys.RightShift, KeyboardIcon.RightShift},
            {Keys.RightAlt, KeyboardIcon.RightAlt},
            {Keys.RightControl, KeyboardIcon.RightCtrl},

            {Keys.Enter, KeyboardIcon.A},
            {Keys.Escape, KeyboardIcon.A},
            {Keys.Tab, KeyboardIcon.A},

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
    }
}