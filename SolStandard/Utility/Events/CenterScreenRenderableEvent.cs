using SolStandard.Containers.Components.World;
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

            WorldContext.WorldHUD.RenderCenterScreen(content);

            if (frameCounter <= frameLifetime) return;

            WorldContext.WorldHUD.StopRenderingCenterScreenContent();
            Complete = true;
        }
    }
}