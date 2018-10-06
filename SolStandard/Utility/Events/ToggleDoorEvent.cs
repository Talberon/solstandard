using SolStandard.Entity.General;

namespace SolStandard.Utility.Events
{
    public class ToggleDoorEvent : IEvent
    {
        private readonly Door targetDoor;
        public bool Complete { get; private set; }

        public ToggleDoorEvent(Door targetDoor)
        {
            this.targetDoor = targetDoor;
            Complete = false;
        }

        public void Continue()
        {
            if (!targetDoor.IsOpen)
            {
                //TODO AssetManager.OpenDoorSFX.Play();
                targetDoor.Open();
            }
            else
            {
                //TODO AssetManager.CloseDoorSFX.Play();
                targetDoor.Close();
            }

            Complete = true;
        }
    }
}