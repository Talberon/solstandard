using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility.Inputs.Joystick
{
    public class JoystickController : IController
    {
        public ControlType ControlType => ControlType.Gamepad;

        public JoystickController(PlayerIndex playerIndex)
        {
            Inputs = new Dictionary<Input, GameControl>
            {
                {Input.None, new VoidInput()},

                {Input.Confirm, new InputJoystick(playerIndex, JoystickButton.BottomFaceButton)},
                {Input.Cancel, new InputJoystick(playerIndex, JoystickButton.RightFaceButton)},
                {Input.PreviewUnit, new InputJoystick(playerIndex, JoystickButton.LeftFaceButton)},
                {Input.PreviewItem, new InputJoystick(playerIndex, JoystickButton.TopFaceButton)},
                {Input.CursorUp, new InputJoystickDPad(playerIndex, Direction.Up)},
                {Input.CursorDown, new InputJoystickDPad(playerIndex, Direction.Down)},
                {Input.CursorLeft, new InputJoystickDPad(playerIndex, Direction.Left)},
                {Input.CursorRight, new InputJoystickDPad(playerIndex, Direction.Right)},
                {Input.CameraUp, new InputJoystickAxis(playerIndex, JoystickAxisInput.RightThumbstickUp)},
                {Input.CameraDown, new InputJoystickAxis(playerIndex, JoystickAxisInput.RightThumbstickDown)},
                {Input.CameraLeft, new InputJoystickAxis(playerIndex, JoystickAxisInput.RightThumbstickLeft)},
                {Input.CameraRight, new InputJoystickAxis(playerIndex, JoystickAxisInput.RightThumbstickRight)},
                {Input.Menu, new InputJoystick(playerIndex, JoystickButton.Start)},
                {Input.Status, new InputJoystick(playerIndex, JoystickButton.Back)},
                {Input.TabLeft, new InputJoystick(playerIndex, JoystickButton.LeftShoulder)},
                {Input.TabRight, new InputJoystick(playerIndex, JoystickButton.RightShoulder)},
                {Input.ZoomOut, new InputJoystickAxis(playerIndex, JoystickAxisInput.LeftTrigger)},
                {Input.ZoomIn, new InputJoystickAxis(playerIndex, JoystickAxisInput.RightTrigger)}
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

        public Dictionary<Input, GameControl> Inputs { get; }

        public GameControl Confirm => Inputs[Input.Confirm];
        public GameControl Cancel => Inputs[Input.Cancel];
        public GameControl PreviewUnit => Inputs[Input.PreviewUnit];
        public GameControl PreviewItem => Inputs[Input.PreviewItem];

        public GameControl CursorUp => Inputs[Input.CursorUp];
        public GameControl CursorDown => Inputs[Input.CursorDown];
        public GameControl CursorLeft => Inputs[Input.CursorLeft];
        public GameControl CursorRight => Inputs[Input.CursorRight];

        public GameControl CameraUp => Inputs[Input.CameraUp];
        public GameControl CameraDown => Inputs[Input.CameraDown];
        public GameControl CameraLeft => Inputs[Input.CameraLeft];
        public GameControl CameraRight => Inputs[Input.CameraRight];

        public GameControl Menu => Inputs[Input.Menu];
        public GameControl Status => Inputs[Input.Status];

        public GameControl SetWideZoom => Inputs[Input.TabLeft];
        public GameControl SetCloseZoom => Inputs[Input.TabRight];
        public GameControl AdjustZoomOut => Inputs[Input.ZoomOut];
        public GameControl AdjustZoomIn => Inputs[Input.ZoomIn];
    }
}