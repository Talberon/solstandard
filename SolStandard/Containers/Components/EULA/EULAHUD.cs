using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;

namespace SolStandard.Containers.Components.EULA
{
    public class EULAHUD : ScrollingTextPaneHUD
    {
        public EULAHUD() : base(AssetManager.WindowFont, AssetManager.EULAText, new WindowContentGrid(
            new IRenderable[,]
            {
                {
                    new WindowContentGrid(new[,]
                    {
                        {
                            new RenderText(AssetManager.WindowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.Confirm, GameDriver.CellSize),
                            new RenderText(AssetManager.WindowFont, " to accept the agreement."),
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