using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility
{
    public static class TeamUtility
    {
        public static Color DetermineTeamColor(Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    return new Color(40, 40, 100, 200);
                case Team.Red:
                    return new Color(100, 35, 35, 200);
                default:
                    return new Color(75, 150, 75, 200);
            }
        }
    }
}