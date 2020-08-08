using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.NeoGFX.Graphics;
using SolStandard.NeoUtility.Controls.Inputs.Prefabs;
using SolStandard.NeoUtility.Monogame.Assets;
using SolStandard.NeoUtility.Monogame.Interfaces;
using SolStandard.Utility.Exceptions;

namespace SolStandard.NeoUtility.Controls.Inputs.Keyboard
{
    public enum KeyboardIndex
    {
        One,
    }

    [Serializable]
    public class KeyboardController : IController
    {
        private readonly KeyboardIndex keyboardIndex;

        public AsepriteWrapper Icon => AssetManager.CombatHudSprite
            .WithAnimation($"ctrl-keyboard{(int) keyboardIndex + 1}").ToWrapper();

        public SpriteAtlas IconForInput(Input input, Vector2 renderSize)
        {
            var key = Inputs[input] as InputKey;
            if (key is object) return KeyboardIconProvider.GetKeyboardIcon(key.InputIcon, renderSize);

            throw new InputNotFoundException(input, this);
        }

        public static IController From(KeyboardController controller)
        {
            return new KeyboardController(
                controller.Inputs[Input.ContextAction],
                controller.Inputs[Input.TertiaryAction],
                controller.Inputs[Input.PrimaryAction],
                controller.Inputs[Input.SecondaryAction],
                controller.Inputs[Input.MoveUp],
                controller.Inputs[Input.MoveDown],
                controller.Inputs[Input.MoveLeft],
                controller.Inputs[Input.MoveRight],
                new VoidInput(),
                new VoidInput(),
                new VoidInput(),
                new VoidInput(),
                controller.Inputs[Input.Select],
                controller.Inputs[Input.Start],
                controller.Inputs[Input.DodgeRoll],
                controller.Inputs[Input.Parry],
                new VoidInput(),
                new VoidInput()
            );
        }

        public Dictionary<Input, GameControl> Inputs { get; }

        public InputDevice InputDevice { get; }
        public ControlType ControlType => ControlType.Keyboard;

        public GameControl Confirm => Inputs[Input.ContextAction];
        public GameControl Cancel => Inputs[Input.TertiaryAction];
        public GameControl SubweaponLeft => Inputs[Input.PrimaryAction];
        public GameControl SubweaponTop => Inputs[Input.SecondaryAction];

        public GameControl MoveUp => Inputs[Input.MoveUp];
        public GameControl MoveDown => Inputs[Input.MoveDown];
        public GameControl MoveLeft => Inputs[Input.MoveLeft];
        public GameControl MoveRight => Inputs[Input.MoveRight];

        public GameControl CameraUp => Inputs[Input.UnusedUp];
        public GameControl CameraDown => Inputs[Input.UnusedDown];
        public GameControl CameraLeft => Inputs[Input.UnusedLeft];
        public GameControl CameraRight => Inputs[Input.UnusedRight];

        public GameControl Start => Inputs[Input.Start];
        public GameControl Select => Inputs[Input.Select];

        public GameControl TabLeft => Inputs[Input.DodgeRoll];
        public GameControl TabRight => Inputs[Input.Parry];
        public GameControl LeftTrigger => Inputs[Input.UnusedLeftTrigger];
        public GameControl RightTrigger => Inputs[Input.UnusedRightTrigger];

        public KeyboardController(KeyboardIndex keyboardIndex)
        {
            this.keyboardIndex = keyboardIndex;
            InputDevice = keyboardIndex switch
            {
                KeyboardIndex.One => InputDevice.Keyboard1,
                _ => throw new ArgumentOutOfRangeException()
            };

            Inputs = new Dictionary<Input, GameControl>
            {
                {Input.None, new VoidInput()},
                {Input.ContextAction, new InputKey(Keys.Space)},
                {Input.TertiaryAction, new InputKey(Keys.LeftShift)},
                {Input.PrimaryAction, new InputKey(Keys.Q)},
                {Input.SecondaryAction, new InputKey(Keys.E)},
                {Input.MoveUp, new InputKey(Keys.W)},
                {Input.MoveDown, new InputKey(Keys.S)},
                {Input.MoveLeft, new InputKey(Keys.A)},
                {Input.MoveRight, new InputKey(Keys.D)},
                {Input.UnusedUp, new InputKey(Keys.Up)},
                {Input.UnusedDown, new InputKey(Keys.Down)},
                {Input.UnusedLeft, new InputKey(Keys.Left)},
                {Input.UnusedRight, new InputKey(Keys.Right)},
                {Input.Select, new InputKey(Keys.Escape)},
                {Input.Start, new InputKey(Keys.Enter)},
                {Input.DodgeRoll, new InputKey(Keys.Tab)},
                {Input.Parry, new InputKey(Keys.R)},
                {Input.UnusedLeftTrigger, new InputKey(Keys.LeftControl)},
                {Input.UnusedRightTrigger, new InputKey(Keys.LeftAlt)}
            };
        }

        private KeyboardController(
            GameControl confirm,
            GameControl cancel,
            GameControl previewUnit,
            GameControl previewItem,
            GameControl cursorUp,
            GameControl cursorDown,
            GameControl cursorLeft,
            GameControl cursorRight,
            GameControl cameraUp,
            GameControl cameraDown,
            GameControl cameraLeft,
            GameControl cameraRight,
            GameControl menu,
            GameControl status,
            GameControl tabLeft,
            GameControl tabRight,
            GameControl zoomOut,
            GameControl zoomIn
        )
        {
            Inputs = new Dictionary<Input, GameControl>
            {
                {Input.None, new VoidInput()},

                {Input.ContextAction, confirm},
                {Input.TertiaryAction, cancel},
                {Input.PrimaryAction, previewUnit},
                {Input.SecondaryAction, previewItem},
                {Input.MoveUp, cursorUp},
                {Input.MoveDown, cursorDown},
                {Input.MoveLeft, cursorLeft},
                {Input.MoveRight, cursorRight},
                {Input.UnusedUp, cameraUp},
                {Input.UnusedDown, cameraDown},
                {Input.UnusedLeft, cameraLeft},
                {Input.UnusedRight, cameraRight},
                {Input.Select, menu},
                {Input.Start, status},
                {Input.DodgeRoll, tabLeft},
                {Input.Parry, tabRight},
                {Input.UnusedLeftTrigger, zoomOut},
                {Input.UnusedRightTrigger, zoomIn}
            };
        }

        public GameControl GetInput(Input input)
        {
            return Inputs[input];
        }

        public void RemapControl(Input inputToRemap, GameControl newInput)
        {
            if (InputAlreadySet(newInput)) throw new DuplicateInputException();

            Inputs[inputToRemap] = newInput;
        }

        private bool InputAlreadySet(GameControl potentialControl)
        {
            return ((Input[]) Enum.GetValues(typeof(Input))).Any(input => Inputs[input] == potentialControl);
        }

        private bool Equals(IController other)
        {
            return Equals(Confirm, other.Confirm) && Equals(Cancel, other.Cancel) &&
                   Equals(SubweaponLeft, other.SubweaponLeft) && Equals(SubweaponTop, other.SubweaponTop) &&
                   Equals(MoveUp, other.MoveUp) && Equals(MoveDown, other.MoveDown) &&
                   Equals(MoveLeft, other.MoveLeft) && Equals(MoveRight, other.MoveRight) &&
                   Equals(CameraUp, other.CameraUp) && Equals(CameraDown, other.CameraDown) &&
                   Equals(CameraLeft, other.CameraLeft) && Equals(CameraRight, other.CameraRight) &&
                   Equals(Start, other.Start) && Equals(Select, other.Select) && Equals(TabLeft, other.TabLeft) &&
                   Equals(TabRight, other.TabRight) && Equals(LeftTrigger, other.LeftTrigger) &&
                   Equals(RightTrigger, other.RightTrigger);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((object) (KeyboardController) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Confirm.GetHashCode());
                hashCode = (hashCode * 397) ^ (Cancel.GetHashCode());
                hashCode = (hashCode * 397) ^ (SubweaponLeft.GetHashCode());
                hashCode = (hashCode * 397) ^ (SubweaponTop.GetHashCode());
                hashCode = (hashCode * 397) ^ (MoveUp.GetHashCode());
                hashCode = (hashCode * 397) ^ (MoveDown.GetHashCode());
                hashCode = (hashCode * 397) ^ (MoveLeft.GetHashCode());
                hashCode = (hashCode * 397) ^ (MoveRight.GetHashCode());
                hashCode = (hashCode * 397) ^ (CameraUp.GetHashCode());
                hashCode = (hashCode * 397) ^ (CameraDown.GetHashCode());
                hashCode = (hashCode * 397) ^ (CameraLeft.GetHashCode());
                hashCode = (hashCode * 397) ^ (CameraRight.GetHashCode());
                hashCode = (hashCode * 397) ^ (Start.GetHashCode());
                hashCode = (hashCode * 397) ^ (Select.GetHashCode());
                hashCode = (hashCode * 397) ^ (TabLeft.GetHashCode());
                hashCode = (hashCode * 397) ^ (TabRight.GetHashCode());
                hashCode = (hashCode * 397) ^ (LeftTrigger.GetHashCode());
                hashCode = (hashCode * 397) ^ (RightTrigger.GetHashCode());
                return hashCode;
            }
        }
    }
}