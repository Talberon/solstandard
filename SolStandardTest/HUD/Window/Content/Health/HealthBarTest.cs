using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace SolStandardTest.HUD.Window.Content.Health
{
    [TestFixture]
    public class HealthBarTest
    {
        [Test]
        public void TestDealOneDamageFromFull()
        {
            const int maxArmor = 5;
            const int maxHp = 5;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxArmor, maxHp, Vector2.One);

            testHealthBar.SetArmorAndHp(4, 5);
            string pipsString = string.Join(",", testHealthBar.HealthPipValues);

            Assert.IsTrue(testHealthBar.GetArmorPips[0].Active, "Armor bar: " + pipsString);
            Assert.IsTrue(testHealthBar.GetArmorPips[1].Active, "Armor bar: " + pipsString);
            Assert.IsTrue(testHealthBar.GetArmorPips[2].Active, "Armor bar: " + pipsString);
            Assert.IsTrue(testHealthBar.GetArmorPips[3].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testHealthBar.GetArmorPips[4].Active, "Armor bar: " + pipsString);

            Assert.IsTrue(testHealthBar.GetHealthPips[0].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testHealthBar.GetHealthPips[1].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testHealthBar.GetHealthPips[2].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testHealthBar.GetHealthPips[3].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testHealthBar.GetHealthPips[4].Active, "Health bar: " + pipsString);
        }

        [Test]
        public void TestDealMultipleDamageFromFull()
        {
            const int maxArmor = 5;
            const int maxHp = 5;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxArmor, maxHp, Vector2.One);

            testHealthBar.SetArmorAndHp(0, 3);
            string pipsString = string.Join(",", testHealthBar.HealthPipValues);

            Assert.IsFalse(testHealthBar.GetArmorPips[0].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testHealthBar.GetArmorPips[1].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testHealthBar.GetArmorPips[2].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testHealthBar.GetArmorPips[3].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testHealthBar.GetArmorPips[4].Active, "Armor bar: " + pipsString);

            Assert.IsTrue(testHealthBar.GetHealthPips[0].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testHealthBar.GetHealthPips[1].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testHealthBar.GetHealthPips[2].Active, "Health bar: " + pipsString);
            Assert.IsFalse(testHealthBar.GetHealthPips[3].Active, "Health bar: " + pipsString);
            Assert.IsFalse(testHealthBar.GetHealthPips[4].Active, "Health bar: " + pipsString);
        }
    }
}