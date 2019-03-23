using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Network;

namespace SolStandard.Utility.Buttons.Network
{
    [Serializable]
    public class NetworkController : IController, ISerializable
    {
        private readonly Dictionary<Input, InputNet> inputs;
        private const string NCPrefix = "NC";

        public NetworkController()
        {
            Confirm = new InputNet();
            Cancel = new InputNet();
            ResetToUnit = new InputNet();
            CenterCamera = new InputNet();

            CursorUp = new InputNet();
            CursorDown = new InputNet();
            CursorLeft = new InputNet();
            CursorRight = new InputNet();

            CameraUp = new InputNet();
            CameraDown = new InputNet();
            CameraLeft = new InputNet();
            CameraRight = new InputNet();

            Menu = new InputNet();
            Status = new InputNet();

            SetWideZoom = new InputNet();
            SetCloseZoom = new InputNet();
            AdjustZoomOut = new InputNet();
            AdjustZoomIn = new InputNet();

            inputs = new Dictionary<Input, InputNet>
            {
                {Input.Confirm, (InputNet) Confirm},
                {Input.Cancel, (InputNet) Cancel},
                {Input.PreviewUnit, (InputNet) ResetToUnit},
                {Input.Y, (InputNet) CenterCamera},

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

        public NetworkController(SerializationInfo info, StreamingContext context) : this()
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

        public IRenderable GetInputIcon(Input input, Vector2 iconSize)
        {
            return new RenderBlank();
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

            info.AddValue(NCPrefix + (int) Input.Confirm, Confirm.Pressed);
            info.AddValue(NCPrefix + (int) Input.Cancel, Cancel.Pressed);
            info.AddValue(NCPrefix + (int) Input.PreviewUnit, ResetToUnit.Pressed);
            info.AddValue(NCPrefix + (int) Input.Y, CenterCamera.Pressed);
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

        public override bool Equals(object networkController)
        {
            NetworkController otherController = networkController as NetworkController;

            if (otherController == null) return false;

            foreach (Input input in Enum.GetValues(typeof(Input)))
            {
                if (input == Input.None) continue;
                if (otherController.GetInput(input).Pressed != inputs[input].Pressed) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (inputs != null ? inputs.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Confirm != null ? Confirm.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Cancel != null ? Cancel.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ResetToUnit != null ? ResetToUnit.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CenterCamera != null ? CenterCamera.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CursorUp != null ? CursorUp.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CursorDown != null ? CursorDown.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CursorLeft != null ? CursorLeft.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CursorRight != null ? CursorRight.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CameraUp != null ? CameraUp.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CameraDown != null ? CameraDown.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CameraLeft != null ? CameraLeft.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CameraRight != null ? CameraRight.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Menu != null ? Menu.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Status != null ? Status.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SetWideZoom != null ? SetWideZoom.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SetCloseZoom != null ? SetCloseZoom.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AdjustZoomOut != null ? AdjustZoomOut.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AdjustZoomIn != null ? AdjustZoomIn.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            string description = "NetworkController {";
            description += Environment.NewLine;
            description += string.Format("<{0}: {1}>, ", Input.Confirm.ToString(), Confirm);
            description += string.Format("<{0}: {1}>, ", Input.Cancel.ToString(), Cancel);
            description += string.Format("<{0}: {1}>, ", Input.PreviewUnit.ToString(), ResetToUnit);
            description += string.Format("<{0}: {1}>, ", Input.Y.ToString(), CenterCamera);
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