using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Pause;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu
{
    public class ReturnToPauseMenuOption : MenuOption
    {
        public static bool FromMainMenu { get; set; }

        public ReturnToPauseMenuOption(Color color) : base(
            new RenderText(AssetManager.WindowFont, "Back"),
            color
        )
        {
        }

        public override void Execute()
        {
            if (FromMainMenu)
            {
                AssetManager.MapUnitSelectSFX.Play();
                GlobalContext.CurrentGameState = GlobalContext.GameState.MainMenu;
            }
            else
            {
                PauseScreenUtils.ChangeMenu(PauseScreenUtils.PauseMenus.Primary);
            }
        }

        public override IRenderable Clone()
        {
            return new ReturnToPauseMenuOption(DefaultColor);
        }
    }
}