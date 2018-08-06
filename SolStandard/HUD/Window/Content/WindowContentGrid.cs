using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content
{
    public class WindowContentGrid
    {
        private readonly IRenderable[,] contentGrid;

        public WindowContentGrid(IRenderable[,] contentGrid)
        {
            this.contentGrid = contentGrid;
        }

        public IRenderable[,] ContentGrid
        {
            get { return contentGrid; }
        }
    }
}