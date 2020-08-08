//TODO Update Menu and implement IGameContexts, then re-enable this

// using System;
// using System.Collections.Generic;
// using Microsoft.Xna.Framework;
// using Steelbreakers.Utility.Controls.Inputs;
// using Steelbreakers.Utility.Juice;
// using Steelbreakers.Utility.Monogame.Assets;
//
// namespace Steelbreakers.Utility.GUI.HUD.Menus.Implementations
// {
//     public class SimpleSliderMenuOption : SliderMenuOption
//     {
//         private readonly IWindowContent icon;
//         private readonly string label;
//         public float CurrentValue { get; private set; }
//         public int CurrentValueInt => (int) Math.Round(CurrentValue);
//         private readonly float minValue;
//         private readonly float maxValue;
//         private readonly float incrementAmount;
//         private readonly float speed;
//
//         public SimpleSliderMenuOption(IEnumerable<ControlMapper> controllers,
//             MenuOption.OnConfirm onConfirm, MenuOption.OnHover onHover, MenuOption.OffHover offHover, IWindowContent icon, string text, float startValue,
//             float minValue, float maxValue, float incrementAmount, Color color, float speed = 0.99f) :
//             base(controllers, Increment, Decrement, onConfirm, onHover, offHover,
//                 GenerateJuicyWindow(icon, text, startValue, color, speed))
//         {
//             label = text;
//             CurrentValue = startValue;
//             this.minValue = minValue;
//             this.maxValue = maxValue;
//             this.incrementAmount = incrementAmount;
//             this.speed = speed;
//             this.icon = icon;
//         }
//
//         private static Window.JuicyWindow GenerateJuicyWindow(IWindowContent icon, string label, float currentValue,
//             Color windowColor, float speed)
//         {
//             return new Window.Builder()
//                 .Content(
//                     new WindowContentGrid.Builder()
//                         .SetContent(icon)
//                         .AddContentToRow(new RenderText(AssetManager.WindowFont, $"< {label}: {currentValue} >"))
//                         .Spacing(5)
//                         .Build()
//                 )
//                 .WindowColor(windowColor)
//                 .Build()
//                 .ToJuicyWindow(speed);
//         }
//
//         private static void UpdateWindow(SimpleSliderMenuOption option)
//         {
//             option.JuicyWindow = GenerateJuicyWindow(
//                 option.icon,
//                 option.label,
//                 option.CurrentValue,
//                 option.JuiceBox.TargetColor,
//                 option.speed
//             );
//         }
//
//         private static void Increment(MenuOption option)
//         {
//             if (!(option is SimpleSliderMenuOption sliderOption)) throw new InvalidCastException();
//
//             if (sliderOption.CurrentValue + sliderOption.incrementAmount > sliderOption.maxValue)
//             {
//                 sliderOption.CurrentValue = sliderOption.maxValue;
//                 SoundBox.MenuMoveEdge.Play();
//             }
//             else
//             {
//                 sliderOption.CurrentValue += sliderOption.incrementAmount;
//                 SoundBox.MenuMove.Play();
//             }
//
//             UpdateWindow(sliderOption);
//             option.Hover(option);
//         }
//
//         private static void Decrement(MenuOption option)
//         {
//             if (!(option is SimpleSliderMenuOption sliderOption)) throw new InvalidCastException();
//
//             if (sliderOption.CurrentValue - sliderOption.incrementAmount < sliderOption.minValue)
//             {
//                 sliderOption.CurrentValue = sliderOption.minValue;
//                 SoundBox.MenuMoveEdge.Play();
//             }
//             else
//             {
//                 sliderOption.CurrentValue -= sliderOption.incrementAmount;
//                 SoundBox.MenuMove.Play();
//             }
//
//             UpdateWindow(sliderOption);
//             option.Hover(option);
//         }
//     }
// }