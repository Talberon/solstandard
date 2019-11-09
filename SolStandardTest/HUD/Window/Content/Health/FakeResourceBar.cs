using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content.Health;
using SolStandard.Utility;
using SolStandardTest.Utility.Monogame;

namespace SolStandardTest.HUD.Window.Content.Health
{
    public class FakeResourceBar : HealthBar
    {
        public FakeResourceBar(int maxHp, int currentHp, Vector2 barSize) : base(maxHp, currentHp, barSize)
        {
        }

        protected override void AddHealthPoint(List<IResourcePoint> points)
        {
            points.Add(
                new ResourcePoint(
                    Vector2.One,
                    new SpriteAtlas(new FakeTexture2D(""), Vector2.One),
                    new SpriteAtlas(new FakeTexture2D(""), Vector2.One)
                )
            );
        }

        protected override void AddArmorPoint(List<IResourcePoint> points)
        {
            AddHealthPoint(points);
        }

        public List<IResourcePoint> GetHealthPips => HealthPips;

        public List<IResourcePoint> GetArmorPips => ArmorPips;

        public IEnumerable<string> HealthPipValues
        {
            get { return GetHealthPips.Select(pip => pip.Active.ToString()).ToList(); }
        }
    }
}