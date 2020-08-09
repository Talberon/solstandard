using SolStandard.Containers.Components.World;
using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class ToggleOpenEvent : IEvent
    {
        private readonly IOpenable openable;
        public bool Complete { get; private set; }

        public ToggleOpenEvent(IOpenable openable)
        {
            this.openable = openable;
            Complete = false;
        }

        public void Continue()
        {
            if (!openable.IsOpen)
            {
                openable.Open();
                WorldContext.WorldHUD.GenerateObjectiveWindow();
            }
            else
            {
                openable.Close();
            }

            Complete = true;
        }
    }
}