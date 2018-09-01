using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers.UI
{
    public static class ColorMapper
    {
        public static Color DetermineTeamColor(Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    return new Color(75, 75, 150, 200);
                case Team.Red:
                    return new Color(150, 75, 75, 200);
                default:
                    return new Color(75, 150, 75, 200);
            }
        }
    }
}