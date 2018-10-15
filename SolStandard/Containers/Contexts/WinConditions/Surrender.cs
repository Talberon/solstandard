namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Surrender : WinCondition
    {
        public bool BlueConcedes { private get; set; }
        public bool RedConcedes { private get; set; }

        public override bool ConditionsMet(GameContext gameContext)
        {
            if (BlueConcedes)
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            if (RedConcedes)
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            return false;
        }
    }
}