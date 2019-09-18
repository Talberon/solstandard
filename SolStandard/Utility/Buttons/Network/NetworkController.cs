using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;
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
                {Input.PreviewItem, (InputNet) CenterCamera},

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

                {Input.TabLeft, (InputNet) SetWideZoom},
                {Input.TabRight, (InputNet) SetCloseZoom},
                {Input.ZoomOut, (InputNet) AdjustZoomOut},
                {Input.ZoomIn, (InputNet) AdjustZoomIn}
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

        public ControlType ControlType => ControlType.Keyboard;

        public GameControl GetInput(Input input)
        {
            return inputs[input];
        }

        public IRenderable GetInputIcon(Input input, Vector2 iconSize)
        {
            return RenderBlank.Blank;
        }

        public GameControl Confirm { get; }
        public GameControl Cancel { get; }
        public GameControl ResetToUnit { get; }
        public GameControl CenterCamera { get; }

        public GameControl CursorUp { get; }
        public GameControl CursorDown { get; }
        public GameControl CursorLeft { get; }
        public GameControl CursorRight { get; }

        public GameControl CameraUp { get; }
        public GameControl CameraDown { get; }
        public GameControl CameraLeft { get; }
        public GameControl CameraRight { get; }

        public GameControl Menu { get; }
        public GameControl Status { get; }

        public GameControl SetWideZoom { get; }
        public GameControl SetCloseZoom { get; }
        public GameControl AdjustZoomOut { get; }
        public GameControl AdjustZoomIn { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(ConnectionManager.PacketTypeHeader, (int) ConnectionManager.PacketType.ControlInput);

            info.AddValue(NCPrefix + (int) Input.Confirm, Confirm.Pressed);
            info.AddValue(NCPrefix + (int) Input.Cancel, Cancel.Pressed);
            info.AddValue(NCPrefix + (int) Input.PreviewUnit, ResetToUnit.Pressed);
            info.AddValue(NCPrefix + (int) Input.PreviewItem, CenterCamera.Pressed);
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
            info.AddValue(NCPrefix + (int) Input.TabLeft, SetWideZoom.Pressed);
            info.AddValue(NCPrefix + (int) Input.TabRight, SetCloseZoom.Pressed);
            info.AddValue(NCPrefix + (int) Input.ZoomOut, AdjustZoomOut.Pressed);
            info.AddValue(NCPrefix + (int) Input.ZoomIn, AdjustZoomIn.Pressed);
        }

        public override bool Equals(object networkController)
        {
            if (!(networkController is NetworkController otherController)) return false;

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
                hashCode = (hashCode * 397) ^ (Confirm?.GetHashCode() ?? 0);
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
            description += $"<{Input.Confirm.ToString()}: {Confirm}>, ";
            description += $"<{Input.Cancel.ToString()}: {Cancel}>, ";
            description += $"<{Input.PreviewUnit.ToString()}: {ResetToUnit}>, ";
            description += $"<{Input.PreviewItem.ToString()}: {CenterCamera}>, ";
            description += Environment.NewLine;
            description += $"<{Input.CursorUp.ToString()}: {CursorUp}>, ";
            description += $"<{Input.CursorDown.ToString()}: {CursorDown}>, ";
            description += $"<{Input.CursorLeft.ToString()}: {CursorLeft}>, ";
            description += $"<{Input.CursorRight.ToString()}: {CursorRight}>, ";
            description += Environment.NewLine;
            description += $"<{Input.CameraUp.ToString()}: {CameraUp}>, ";
            description += $"<{Input.CameraDown.ToString()}: {CameraDown}>, ";
            description += $"<{Input.CameraLeft.ToString()}: {CameraLeft}>, ";
            description += $"<{Input.CameraRight.ToString()}: {CameraRight}>, ";
            description += Environment.NewLine;
            description += $"<{Input.Menu.ToString()}: {Menu}>, ";
            description += $"<{Input.Status.ToString()}: {Status}>, ";
            description += Environment.NewLine;
            description += $"<{Input.TabLeft.ToString()}: {SetWideZoom}>, ";
            description += $"<{Input.TabRight.ToString()}: {SetCloseZoom}>, ";
            description += $"<{Input.ZoomOut.ToString()}: {AdjustZoomOut}>, ";
            description += $"<{Input.ZoomIn.ToString()}: {AdjustZoomIn}>";
            description += Environment.NewLine;
            description += "}";

            return description;
        }
    }
}