using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class PreviewUnitSkillsEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.ShowUnitCodexEntry();
            Complete = true;
        }
    }
}