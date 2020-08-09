using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;

namespace SolStandard.Containers.Components.Credits
{
    public class CreditsHUD : ScrollingTextPaneHUD
    {
        public CreditsHUD() : base(AssetManager.WindowFont, AssetManager.CreditsText, new WindowContentGrid(
            new IRenderable[,]
            {
                {
                    new WindowContentGrid(new[,]
                    {
                        {
                            new RenderText(AssetManager.WindowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.Confirm, GameDriver.CellSize),
                            new RenderText(AssetManager.WindowFont, " to view credits in browser."),
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.Cancel, GameDriver.CellSize),
                            new RenderText(AssetManager.WindowFont, " to return to menu."),
                        }
                    })
                },
                {
                    new WindowContentGrid(new[,]
                    {
                        {
                            new RenderText(AssetManager.WindowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.CursorUp, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorLeft, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorDown, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorRight, GameDriver.CellSize),
                            new RenderText(AssetManager.WindowFont, " to scroll."),
                        }
                    })
                }
            }))
        {
        }
    }
}