using SolStandard.Entity;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Events
{
    public class ToggleOpenEvent : IEvent
    {
        private readonly IOpenable openable;
        private readonly ISoundEffect openEffect;
        private readonly ISoundEffect closeEffect;
        public bool Complete { get; private set; }

        public ToggleOpenEvent(IOpenable openable, ISoundEffect openEffect, ISoundEffect closeEffect)
        {
            this.openable = openable;
            this.openEffect = openEffect;
            this.closeEffect = closeEffect;
            Complete = false;
        }

        public void Continue()
        {
            if (!openable.IsOpen)
            {
                openEffect.Play();
                openable.Open();
            }
            else
            {
                closeEffect.Play();
                openable.Close();
            }

            Complete = true;
        }
    }
}