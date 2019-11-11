using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility
{
    public static class TeamUtility
    {
        public static Color DetermineTeamColor(Team team)
        {
            return team switch
            {
                Team.Blue => new Color(40, 40, 100, 200),
                Team.Red => new Color(100, 35, 35, 200),
                _ => new Color(35, 100, 35, 200)
            };
        }
    }
}