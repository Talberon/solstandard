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
            var testResourceBar = new FakeResourceBar(maxArmor, maxHp, Vector2.One);

            testResourceBar.SetArmorAndHp(4, 5);
            string pipsString = string.Join(",", testResourceBar.HealthPipValues);

            Assert.IsTrue(testResourceBar.GetArmorPips[0].Active, "Armor bar: " + pipsString);
            Assert.IsTrue(testResourceBar.GetArmorPips[1].Active, "Armor bar: " + pipsString);
            Assert.IsTrue(testResourceBar.GetArmorPips[2].Active, "Armor bar: " + pipsString);
            Assert.IsTrue(testResourceBar.GetArmorPips[3].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testResourceBar.GetArmorPips[4].Active, "Armor bar: " + pipsString);

            Assert.IsTrue(testResourceBar.GetHealthPips[0].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testResourceBar.GetHealthPips[1].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testResourceBar.GetHealthPips[2].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testResourceBar.GetHealthPips[3].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testResourceBar.GetHealthPips[4].Active, "Health bar: " + pipsString);
        }

        [Test]
        public void TestDealMultipleDamageFromFull()
        {
            const int maxArmor = 5;
            const int maxHp = 5;
            var testResourceBar = new FakeResourceBar(maxArmor, maxHp, Vector2.One);

            testResourceBar.SetArmorAndHp(0, 3);
            string pipsString = string.Join(",", testResourceBar.HealthPipValues);

            Assert.IsFalse(testResourceBar.GetArmorPips[0].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testResourceBar.GetArmorPips[1].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testResourceBar.GetArmorPips[2].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testResourceBar.GetArmorPips[3].Active, "Armor bar: " + pipsString);
            Assert.IsFalse(testResourceBar.GetArmorPips[4].Active, "Armor bar: " + pipsString);

            Assert.IsTrue(testResourceBar.GetHealthPips[0].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testResourceBar.GetHealthPips[1].Active, "Health bar: " + pipsString);
            Assert.IsTrue(testResourceBar.GetHealthPips[2].Active, "Health bar: " + pipsString);
            Assert.IsFalse(testResourceBar.GetHealthPips[3].Active, "Health bar: " + pipsString);
            Assert.IsFalse(testResourceBar.GetHealthPips[4].Active, "Health bar: " + pipsString);
        }
    }
}