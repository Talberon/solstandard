using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Exceptions;
using Steelbreakers.Utility.Controls.Inputs.Prefabs;
using Steelbreakers.Utility.Graphics;
using Steelbreakers.Utility.Monogame.Assets;
using Steelbreakers.Utility.Monogame.Interfaces;

namespace Steelbreakers.Utility.Controls.Inputs.Gamepad
{
    [Serializable]
    public class GamepadController : IController
    {
        public readonly PlayerIndex PlayerIndex;


        public AsepriteWrapper Icon =>
            AssetManager.CombatHudSprite.WithAnimation($"ctrl-gamepad{(int) PlayerIndex + 1}").ToWrapper();

        public SpriteAtlas IconForInput(Input input, Vector2 renderSize)
        {
            var button = Inputs[input] as InputButton;
            if (button is object) return GamepadIconProvider.GetButton(button.InputIcon, renderSize);

            throw new InputNotFoundException(input, this);
        }

        public static IController From(GamepadController controller)
        {
            return new GamepadController(
                controller.PlayerIndex,
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

        public ControlType ControlType => ControlType.Gamepad;
        public InputDevice InputDevice { get; }

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

        public GamepadController(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;

            InputDevice = playerIndex switch
            {
                PlayerIndex.One => InputDevice.Gamepad1,
                PlayerIndex.Two => InputDevice.Gamepad2,
                _ => throw new ArgumentOutOfRangeException()
            };

            Inputs = new Dictionary<Input, GameControl>
            {
                {Input.None, new VoidInput()},

                {Input.ContextAction, new InputButton(playerIndex, Buttons.A)},
                {Input.TertiaryAction, new InputButton(playerIndex, Buttons.B)},
                {Input.PrimaryAction, new InputButton(playerIndex, Buttons.X)},
                {Input.SecondaryAction, new InputButton(playerIndex, Buttons.Y)},
                {Input.MoveUp, new InputButton(playerIndex, Buttons.DPadUp)},
                {Input.MoveDown, new InputButton(playerIndex, Buttons.DPadDown)},
                {Input.MoveLeft, new InputButton(playerIndex, Buttons.DPadLeft)},
                {Input.MoveRight, new InputButton(playerIndex, Buttons.DPadRight)},
                {Input.UnusedUp, new InputButton(playerIndex, Buttons.RightThumbstickUp)},
                {Input.UnusedDown, new InputButton(playerIndex, Buttons.RightThumbstickDown)},
                {Input.UnusedLeft, new InputButton(playerIndex, Buttons.RightThumbstickLeft)},
                {Input.UnusedRight, new InputButton(playerIndex, Buttons.RightThumbstickRight)},
                {Input.Select, new InputButton(playerIndex, Buttons.Back)},
                {Input.Start, new InputButton(playerIndex, Buttons.Start)},
                {Input.DodgeRoll, new InputButton(playerIndex, Buttons.LeftShoulder)},
                {Input.Parry, new InputButton(playerIndex, Buttons.RightShoulder)},
                {Input.UnusedLeftTrigger, new InputButton(playerIndex, Buttons.LeftTrigger)},
                {Input.UnusedRightTrigger, new InputButton(playerIndex, Buttons.RightTrigger)}
            };
        }

        private GamepadController(
            PlayerIndex playerIndex,
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
            PlayerIndex = playerIndex;

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
            return Equals((object) (GamepadController) obj);
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