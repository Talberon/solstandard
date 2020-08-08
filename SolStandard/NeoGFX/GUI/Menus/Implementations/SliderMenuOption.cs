//TODO Update Menu and implement IGameContexts, then re-enable this

// using System.Collections.Generic;
// using Microsoft.Xna.Framework;
// using SolStandard;
// using Steelbreakers.Utility.Controls.Inputs;
//
// namespace Steelbreakers.Utility.GUI.HUD.Menus.Implementations
// {
//     public class SliderMenuOption : MenuOption
//     {
//         public delegate void OnIncrement(SliderMenuOption option);
//
//         public delegate void OnDecrement(SliderMenuOption option);
//
//         private readonly IEnumerable<ControlMapper> controllers;
//         private readonly OnIncrement onIncrement;
//         private readonly OnDecrement onDecrement;
//
//         private bool IsActive => GameDriver.ActiveContext.MenuContainer.CurrentOption.Equals(this);
//
//         public SliderMenuOption(IEnumerable<ControlMapper> controllers,
//             OnIncrement onIncrement, OnDecrement onDecrement, OnConfirm onConfirm, OnHover onHover, OffHover offHover,
//             string text, Color windowColor, float speed = 0.99f) :
//             base(onConfirm, onHover, offHover, text, windowColor, speed)
//         {
//             this.controllers = controllers;
//             this.onIncrement = onIncrement;
//             this.onDecrement = onDecrement;
//         }
//
//         public SliderMenuOption(IEnumerable<ControlMapper> controllers,
//             OnIncrement onIncrement, OnDecrement onDecrement,
//             OnConfirm onConfirm, OnHover onHover, OffHover offHover,
//             Window.JuicyWindow juicyWindow) :
//             base(onConfirm, onHover, offHover, juicyWindow)
//         {
//             this.controllers = controllers;
//             this.onIncrement = onIncrement;
//             this.onDecrement = onDecrement;
//         }
//
//         public override void Update(GameTime gameTime)
//         {
//             if (IsActive)
//             {
//                 foreach (ControlMapper controller in controllers)
//                 {
//                     //HACK. Ensure that slider options do not appear on grid menus or this will get janky.
//                     if (controller.Peek(Input.MoveRight, PressType.DelayedRepeat))
//                     {
//                         onIncrement.Invoke(this);
//                     }
//
//                     if (controller.Peek(Input.MoveLeft, PressType.DelayedRepeat))
//                     {
//                         onDecrement.Invoke(this);
//                     }
//                 }
//             }
//
//
//             base.Update(gameTime);
//         }
//     }
// }