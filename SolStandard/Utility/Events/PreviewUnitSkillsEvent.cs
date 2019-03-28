using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events
{
    public class PreviewUnitSkillsEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.GameMapContext.ShowUnitCodexEntry();
            Complete = true;
        }
    }
}