using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility
{
    public static class TeamUtility
    {
        public static Color DetermineTeamWindowColor(Team team)
        {
            return team switch
            {
                Team.Blue => new Color(40, 40, 100, 200),
                Team.Red => new Color(100, 35, 35, 200),
                _ => new Color(35, 100, 35, 200)
            };
        }
        
        public static Color DetermineTeamCursorColor(Team team)
        {
            return team switch
            {
                Team.Blue => new Color(150, 180, 240),
                Team.Red => new Color(240, 120, 120),
                _ => new Color(180, 240, 180)
            };
        }
    }
}