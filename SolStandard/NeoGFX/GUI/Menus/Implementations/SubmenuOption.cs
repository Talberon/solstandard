//TODO Update Menu and implement IGameContexts, then re-enable this

// using Microsoft.Xna.Framework;
// using SolStandard;
// using Steelbreakers.Utility.Juice;
// using Steelbreakers.Utility.Monogame.Assets;
//
// namespace Steelbreakers.Utility.GUI.HUD.Menus.Implementations
// {
//     public class SubmenuOption : MenuOption
//     {
//         public SubmenuOption(Menu menuToOpen, OnHover onHover, OffHover offHover,
//             string text, Color windowColor, OnConfirm? onConfirm = null)
//             : base(OpenSubmenu(menuToOpen, onConfirm), onHover, offHover, text, windowColor)
//         {
//         }
//
//         public SubmenuOption(Menu menuToOpen, OnHover onHover, OffHover offHover,
//             IWindowContent icon, string text, Color windowColor, OnConfirm? onConfirm = null, float speed = 0.99f)
//             : base(OpenSubmenu(menuToOpen, onConfirm), onHover, offHover,
//                 GenerateJuicyWindow(icon, text, windowColor, speed))
//         {
//         }
//
//         private static Window.JuicyWindow GenerateJuicyWindow(IWindowContent icon, string label, Color windowColor,
//             float speed)
//         {
//             return new Window.Builder()
//                 .Content(
//                     new WindowContentGrid.Builder()
//                         .SetContent(icon)
//                         .AddContentToRow(new RenderText(AssetManager.WindowFont, label))
//                         .Spacing(5)
//                         .Build()
//                 )
//                 .WindowColor(windowColor)
//                 .Build()
//                 .ToJuicyWindow(speed);
//         }
//
//         private static OnConfirm OpenSubmenu(Menu menuToOpen, OnConfirm? onConfirm) =>
//             (_) =>
//             {
//                 SoundBox.MainMenuConfirm.Play();
//                 GameDriver.ActiveContext.MenuContainer.OpenSubmenu(menuToOpen);
//                 onConfirm?.Invoke(_);
//             };
//     }
// }