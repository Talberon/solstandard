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

        public new IResourcePoint[] Pips
        {
            get { return base.Pips; }
        }
        
        public List<string> PipValues
        {
            get
            {
                return Pips.Select(pip => pip.Active.ToString()).ToList();
            }
        }
    }
}