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
        private Dictionary<Input, InputNet> inputs;

        public NetworkController(PlayerIndex playerIndex)
        {
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

        public NetworkController(SerializationInfo info, StreamingContext context)
        {
            Confirm = (InputNet) info.GetValue(Input.Confirm.ToString(), typeof(InputNet));
            Cancel = (InputNet) info.GetValue(Input.Cancel.ToString(), typeof(InputNet));
            ResetToUnit = (InputNet) info.GetValue(Input.ResetToUnit.ToString(), typeof(InputNet));
            CenterCamera = (InputNet) info.GetValue(Input.CenterCamera.ToString(), typeof(InputNet));

            CursorUp = (InputNet) info.GetValue(Input.CursorUp.ToString(), typeof(InputNet));
            CursorDown = (InputNet) info.GetValue(Input.CursorDown.ToString(), typeof(InputNet));
            CursorLeft = (InputNet) info.GetValue(Input.CursorLeft.ToString(), typeof(InputNet));
            CursorRight = (InputNet) info.GetValue(Input.CursorRight.ToString(), typeof(InputNet));

            CameraUp = (InputNet) info.GetValue(Input.CameraUp.ToString(), typeof(InputNet));
            CameraDown = (InputNet) info.GetValue(Input.CameraDown.ToString(), typeof(InputNet));
            CameraLeft = (InputNet) info.GetValue(Input.CameraLeft.ToString(), typeof(InputNet));
            CameraRight = (InputNet) info.GetValue(Input.CameraRight.ToString(), typeof(InputNet));

            Menu = (InputNet) info.GetValue(Input.Menu.ToString(), typeof(InputNet));
            Status = (InputNet) info.GetValue(Input.Status.ToString(), typeof(InputNet));

            SetWideZoom = (InputNet) info.GetValue(Input.LeftBumper.ToString(), typeof(InputNet));
            SetCloseZoom = (InputNet) info.GetValue(Input.RightBumper.ToString(), typeof(InputNet));
            AdjustZoomOut = (InputNet) info.GetValue(Input.LeftTrigger.ToString(), typeof(InputNet));
            AdjustZoomIn = (InputNet) info.GetValue(Input.RightTrigger.ToString(), typeof(InputNet));
        }

        public void Press(Input input)
        {
            inputs[input].Press();
        }

        public void Release(Input input)
        {
            inputs[input].Release();
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
            info.AddValue(ConnectionManager.PacketTypeHeader, ConnectionManager.PacketType.ControlInput);

            info.AddValue(Input.Confirm.ToString(), Confirm);
            info.AddValue(Input.Cancel.ToString(), Cancel);
            info.AddValue(Input.ResetToUnit.ToString(), ResetToUnit);
            info.AddValue(Input.CenterCamera.ToString(), CenterCamera);

            info.AddValue(Input.CursorUp.ToString(), CursorUp);
            info.AddValue(Input.CursorDown.ToString(), CursorDown);
            info.AddValue(Input.CursorLeft.ToString(), CursorLeft);
            info.AddValue(Input.CursorRight.ToString(), CursorRight);

            info.AddValue(Input.CameraUp.ToString(), CameraUp);
            info.AddValue(Input.CameraDown.ToString(), CameraDown);
            info.AddValue(Input.CameraLeft.ToString(), CameraLeft);
            info.AddValue(Input.CameraRight.ToString(), CameraRight);

            info.AddValue(Input.Menu.ToString(), Menu);
            info.AddValue(Input.Status.ToString(), Status);

            info.AddValue(Input.LeftBumper.ToString(), SetWideZoom);
            info.AddValue(Input.RightBumper.ToString(), SetCloseZoom);
            info.AddValue(Input.LeftTrigger.ToString(), AdjustZoomOut);
            info.AddValue(Input.RightTrigger.ToString(), AdjustZoomIn);
        }

        public override string ToString()
        {
            string description = "NetworkController {";
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