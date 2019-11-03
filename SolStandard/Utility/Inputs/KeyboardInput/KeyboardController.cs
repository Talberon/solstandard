using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility.Inputs.KeyboardInput
{
    [Serializable]
    public class KeyboardController : IController
    {
        public static IController From(KeyboardController controller)
        {
            return new KeyboardController(
                controller.Inputs[Input.Confirm],
                controller.Inputs[Input.Cancel],
                controller.Inputs[Input.PreviewUnit],
                controller.Inputs[Input.PreviewItem],
                controller.Inputs[Input.CursorUp],
                controller.Inputs[Input.CursorDown],
                controller.Inputs[Input.CursorLeft],
                controller.Inputs[Input.CursorRight],
                controller.Inputs[Input.CameraUp],
                controller.Inputs[Input.CameraDown],
                controller.Inputs[Input.CameraLeft],
                controller.Inputs[Input.CameraRight],
                controller.Inputs[Input.Menu],
                controller.Inputs[Input.Status],
                controller.Inputs[Input.TabLeft],
                controller.Inputs[Input.TabRight],
                controller.Inputs[Input.ZoomOut],
                controller.Inputs[Input.ZoomIn]
            );
        }

        public Dictionary<Input, GameControl> Inputs { get; }

        public ControlType ControlType => ControlType.Keyboard;

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

        public KeyboardController()
        {
            Inputs = new Dictionary<Input, GameControl>
            {
                {Input.None, new VoidInput()},
                {Input.Confirm, new InputKey(Keys.Space)},
                {Input.Cancel, new InputKey(Keys.LeftShift)},
                {Input.PreviewUnit, new InputKey(Keys.Q)},
                {Input.PreviewItem, new InputKey(Keys.E)},
                {Input.CursorUp, new InputKey(Keys.W)},
                {Input.CursorDown, new InputKey(Keys.S)},
                {Input.CursorLeft, new InputKey(Keys.A)},
                {Input.CursorRight, new InputKey(Keys.D)},
                {Input.CameraUp, new InputKey(Keys.Up)},
                {Input.CameraDown, new InputKey(Keys.Down)},
                {Input.CameraLeft, new InputKey(Keys.Left)},
                {Input.CameraRight, new InputKey(Keys.Right)},
                {Input.Menu, new InputKey(Keys.Enter)},
                {Input.Status, new InputKey(Keys.Escape)},
                {Input.TabLeft, new InputKey(Keys.Tab)},
                {Input.TabRight, new InputKey(Keys.R)},
                {Input.ZoomOut, new InputKey(Keys.LeftControl)},
                {Input.ZoomIn, new InputKey(Keys.LeftAlt)}
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

                {Input.Confirm, confirm},
                {Input.Cancel, cancel},
                {Input.PreviewUnit, previewUnit},
                {Input.PreviewItem, previewItem},
                {Input.CursorUp, cursorUp},
                {Input.CursorDown, cursorDown},
                {Input.CursorLeft, cursorLeft},
                {Input.CursorRight, cursorRight},
                {Input.CameraUp, cameraUp},
                {Input.CameraDown, cameraDown},
                {Input.CameraLeft, cameraLeft},
                {Input.CameraRight, cameraRight},
                {Input.Menu, menu},
                {Input.Status, status},
                {Input.TabLeft, tabLeft},
                {Input.TabRight, tabRight},
                {Input.ZoomOut, zoomOut},
                {Input.ZoomIn, zoomIn}
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
                   Equals(PreviewUnit, other.PreviewUnit) && Equals(PreviewItem, other.PreviewItem) &&
                   Equals(CursorUp, other.CursorUp) && Equals(CursorDown, other.CursorDown) &&
                   Equals(CursorLeft, other.CursorLeft) && Equals(CursorRight, other.CursorRight) &&
                   Equals(CameraUp, other.CameraUp) && Equals(CameraDown, other.CameraDown) &&
                   Equals(CameraLeft, other.CameraLeft) && Equals(CameraRight, other.CameraRight) &&
                   Equals(Menu, other.Menu) && Equals(Status, other.Status) && Equals(SetWideZoom, other.SetWideZoom) &&
                   Equals(SetCloseZoom, other.SetCloseZoom) && Equals(AdjustZoomOut, other.AdjustZoomOut) &&
                   Equals(AdjustZoomIn, other.AdjustZoomIn);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KeyboardController) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Confirm != null ? Confirm.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Cancel != null ? Cancel.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PreviewUnit != null ? PreviewUnit.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PreviewItem != null ? PreviewItem.GetHashCode() : 0);
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
    }
}