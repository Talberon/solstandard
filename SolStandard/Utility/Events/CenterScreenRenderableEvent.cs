using SolStandard.Containers.Contexts;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Events
{
    public class CenterScreenRenderableEvent : IEvent
    {
        private readonly IRenderable content;
        private readonly int frameLifetime;
        private readonly ISoundEffect soundEffect;
        private int frameCounter;
        public bool Complete { get; private set; }

        public CenterScreenRenderableEvent(IRenderable content, int frameLifetime, ISoundEffect soundEffect = null)
        {
            this.content = content;
            this.frameLifetime = frameLifetime;
            this.soundEffect = soundEffect;
            frameCounter = 0;
        }

        public void Continue()
        {
            if (frameCounter == 0) soundEffect?.Play();
            
            frameCounter++;

            GameMapContext.GameMapView.RenderCenterScreen(content);

            if (frameCounter <= frameLifetime) return;

            GameMapContext.GameMapView.StopRenderingCenterScreenContent();
            Complete = true;
        }
    }
}