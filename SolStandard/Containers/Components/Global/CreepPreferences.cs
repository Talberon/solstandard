namespace SolStandard.Containers.Components.Global
{
    public class CreepPreferences
    {
        private const string CreepPreferencesFileName = "CreepSpawn";

        public bool CreepsCanSpawn { get; private set; }

        public CreepPreferences()
        {
            LoadSavedPreferences();
        }

        private void LoadSavedPreferences()
        {
            if (GameDriver.FileIO.FileExists(CreepPreferencesFileName))
            {
                CreepsCanSpawn = GameDriver.FileIO.Load<bool>(CreepPreferencesFileName);
            }
            else
            {
                CreepsCanSpawn = true;
                GameDriver.FileIO.Save(CreepPreferencesFileName, CreepsCanSpawn);
            }
        }

        public void ToggleCreepsCanSpawn()
        {
            CreepsCanSpawn = !CreepsCanSpawn;
            GameDriver.FileIO.Save(CreepPreferencesFileName, CreepsCanSpawn);
        }
    }
}