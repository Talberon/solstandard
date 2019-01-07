using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Buttons.Gamepad;
using SolStandard.Utility.Network;

namespace SolStandard.Utility.Buttons.Network
{
    [Serializable]
    public class NetworkController : IController, ISerializable
    {
        private readonly Dictionary<Input, InputNet> inputs;
        private readonly PlayerIndex playerIndex;
        private const string NCPrefix = "NC";

        public NetworkController(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;

            Confirm = new InputNet(playerIndex);
            Cancel = new InputNet(playerIndex);
            ResetToUnit = new InputNet(playerIndex);
            CenterCamera = new InputNet(playerIndex);

            CursorUp = new InputNet(playerIndex);
            CursorDown = new InputNet(playerIndex);
            CursorLeft = new InputNet(playerIndex);
            CursorRight = new InputNet(playerIndex);

            CameraUp = new InputNet(playerIndex);
            CameraDown = new InputNet(playerIndex);
            CameraLeft = new InputNet(playerIndex);
            CameraRight = new InputNet(playerIndex);

            Menu = new InputNet(playerIndex);
            Status = new InputNet(playerIndex);

            SetWideZoom = new InputNet(playerIndex);
            SetCloseZoom = new InputNet(playerIndex);
            AdjustZoomOut = new InputNet(playerIndex);
            AdjustZoomIn = new InputNet(playerIndex);

            inputs = new Dictionary<Input, InputNet>
            {
                {Input.Confirm, (InputNet) Confirm},
                {Input.Cancel, (InputNet) Cancel},
                {Input.ResetToUnit, (InputNet) ResetToUnit},
                {Input.CenterCamera, (InputNet) CenterCamera},

                {Input.CursorUp, (InputNet) CursorUp},
                {Input.CursorDown, (InputNet) CursorDown},
                {Input.CursorLeft, (InputNet) CursorLeft},
                {Input.CursorRight, (InputNet) CursorRight},

                {Input.CameraUp, (InputNet) CameraUp},
                {Input.CameraDown, (InputNet) CameraDown},
                {Input.CameraLeft, (InputNet) CameraLeft},
                {Input.CameraRight, (InputNet) CameraRight},

                {Input.Menu, (InputNet) Menu},
                {Input.Status, (InputNet) Status},

                {Input.LeftBumper, (InputNet) SetWideZoom},
                {Input.RightBumper, (InputNet) SetCloseZoom},
                {Input.LeftTrigger, (InputNet) AdjustZoomOut},
                {Input.RightTrigger, (InputNet) AdjustZoomIn},
            };
        }

        public NetworkController(SerializationInfo info, StreamingContext context) : this(PlayerIndex.One)
        {
            foreach (Input input in Enum.GetValues(typeof(Input)))
            {
                if (input == Input.None) continue;

                //Extract each GameControl type based on the prefix plus the integer value of the enum, e.g. NC12
                bool pressed = (bool) info.GetValue(NCPrefix + (int) input, typeof(bool));

                if (pressed)
                {
                    inputs[input].Press();
                }
                else
                {
                    inputs[input].Release();
                }
            }
        }

        public void Press(Input input)
        {
            inputs[input].Press();
        }

        public void Release(Input input)
        {
            inputs[input].Release();
        }

        public GameControl GetInput(Input input)
        {
            return inputs[input];
        }

        public void MimicInput(IController controllerToMimic)
        {
            foreach (Input input in Enum.GetValues(typeof(Input)))
            {
                if (input == Input.None) continue;

                if (controllerToMimic.GetInput(input).Pressed)
                {
                    Press(input);
                }
                else
                {
                    Release(input);
                }
            }
        }

        public GameControl Confirm { get; private set; }
        public GameControl Cancel { get; private set; }
        public GameControl ResetToUnit { get; private set; }
        public GameControl CenterCamera { get; private set; }

        public GameControl CursorUp { get; private set; }
        public GameControl CursorDown { get; private set; }
        public GameControl CursorLeft { get; private set; }
        public GameControl CursorRight { get; private set; }

        public GameControl CameraUp { get; private set; }
        public GameControl CameraDown { get; private set; }
        public GameControl CameraLeft { get; private set; }
        public GameControl CameraRight { get; private set; }

        public GameControl Menu { get; private set; }
        public GameControl Status { get; private set; }

        public GameControl SetWideZoom { get; private set; }
        public GameControl SetCloseZoom { get; private set; }
        public GameControl AdjustZoomOut { get; private set; }
        public GameControl AdjustZoomIn { get; private set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(ConnectionManager.PacketTypeHeader, (int) ConnectionManager.PacketType.ControlInput);
            info.AddValue("PLAYER", (int) playerIndex);

            info.AddValue(NCPrefix + (int) Input.Confirm, Confirm.Pressed);
            info.AddValue(NCPrefix + (int) Input.Cancel, Cancel.Pressed);
            info.AddValue(NCPrefix + (int) Input.ResetToUnit, ResetToUnit.Pressed);
            info.AddValue(NCPrefix + (int) Input.CenterCamera, CenterCamera.Pressed);
            info.AddValue(NCPrefix + (int) Input.CursorUp, CursorUp.Pressed);
            info.AddValue(NCPrefix + (int) Input.CursorDown, CursorDown.Pressed);
            info.AddValue(NCPrefix + (int) Input.CursorLeft, CursorLeft.Pressed);
            info.AddValue(NCPrefix + (int) Input.CursorRight, CursorRight.Pressed);
            info.AddValue(NCPrefix + (int) Input.CameraUp, CameraUp.Pressed);
            info.AddValue(NCPrefix + (int) Input.CameraDown, CameraDown.Pressed);
            info.AddValue(NCPrefix + (int) Input.CameraLeft, CameraLeft.Pressed);
            info.AddValue(NCPrefix + (int) Input.CameraRight, CameraRight.Pressed);
            info.AddValue(NCPrefix + (int) Input.Menu, Menu.Pressed);
            info.AddValue(NCPrefix + (int) Input.Status, Status.Pressed);
            info.AddValue(NCPrefix + (int) Input.LeftBumper, SetWideZoom.Pressed);
            info.AddValue(NCPrefix + (int) Input.RightBumper, SetCloseZoom.Pressed);
            info.AddValue(NCPrefix + (int) Input.LeftTrigger, AdjustZoomOut.Pressed);
            info.AddValue(NCPrefix + (int) Input.RightTrigger, AdjustZoomIn.Pressed);
        }

        public override string ToString()
        {
            string description = "NetworkController {";
            description += Environment.NewLine;
            description += string.Format("Player Index: {0}", playerIndex);
            description += Environment.NewLine;
            description += string.Format("<{0}: {1}>, ", Input.Confirm.ToString(), Confirm);
            description += string.Format("<{0}: {1}>, ", Input.Cancel.ToString(), Cancel);
            description += string.Format("<{0}: {1}>, ", Input.ResetToUnit.ToString(), ResetToUnit);
            description += string.Format("<{0}: {1}>, ", Input.CenterCamera.ToString(), CenterCamera);
            description += Environment.NewLine;
            description += string.Format("<{0}: {1}>, ", Input.CursorUp.ToString(), CursorUp);
            description += string.Format("<{0}: {1}>, ", Input.CursorDown.ToString(), CursorDown);
            description += string.Format("<{0}: {1}>, ", Input.CursorLeft.ToString(), CursorLeft);
            description += string.Format("<{0}: {1}>, ", Input.CursorRight.ToString(), CursorRight);
            description += Environment.NewLine;
            description += string.Format("<{0}: {1}>, ", Input.CameraUp.ToString(), CameraUp);
            description += string.Format("<{0}: {1}>, ", Input.CameraDown.ToString(), CameraDown);
            description += string.Format("<{0}: {1}>, ", Input.CameraLeft.ToString(), CameraLeft);
            description += string.Format("<{0}: {1}>, ", Input.CameraRight.ToString(), CameraRight);
            description += Environment.NewLine;
            description += string.Format("<{0}: {1}>, ", Input.Menu.ToString(), Menu);
            description += string.Format("<{0}: {1}>, ", Input.Status.ToString(), Status);
            description += Environment.NewLine;
            description += string.Format("<{0}: {1}>, ", Input.LeftBumper.ToString(), SetWideZoom);
            description += string.Format("<{0}: {1}>, ", Input.RightBumper.ToString(), SetCloseZoom);
            description += string.Format("<{0}: {1}>, ", Input.LeftTrigger.ToString(), AdjustZoomOut);
            description += string.Format("<{0}: {1}>", Input.RightTrigger.ToString(), AdjustZoomIn);
            description += Environment.NewLine;
            description += "}";

            return description;
        }
    }
}