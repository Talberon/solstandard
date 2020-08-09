using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events
{
    public class PreviewUnitSkillsEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            GlobalContext.WorldContext.ShowUnitCodexEntry();
            Complete = true;
        }
    }
}