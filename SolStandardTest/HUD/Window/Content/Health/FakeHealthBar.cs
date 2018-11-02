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

        public new IResourcePoint[] HealthPips
        {
            get { return base.HealthPips; }
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