using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class WaitActionEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            AssetManager.MapUnitSelectSFX.Play();
            Complete = true;
        }
    }
}