using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content.Health;

namespace SolStandardTest.HUD.Window.Content.Health
{
    public class FakeHealthBar : HealthBar
    {

        public FakeHealthBar(int maxHp, int currentHp, Vector2 barSize) : base(maxHp, currentHp, barSize)
        {
        }

        public List<IResourcePoint> HealthPips
        {
            get { return new List<IResourcePoint>(); }
        }
        
        public List<string> PipValues
        {
            get
            {
                return HealthPips.Select(pip => pip.Active.ToString()).ToList();
            }
        }
    }
}