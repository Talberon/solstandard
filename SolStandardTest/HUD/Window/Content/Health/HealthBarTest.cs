using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace SolStandardTest.HUD.Window.Content.Health
{
    [TestClass]
    public class HealthBarTest
    {
        [TestMethod]
        public void TestDealOneDamageFromFull()
        {
            int maxHp = 5;
            int hp = 5;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxHp, hp, Vector2.One);

            testHealthBar.Update(0, 1);
            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.HealthPips[4].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[3].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[2].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[1].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[0].Active, pipsString);
        }

        [TestMethod]
        public void TestDealMultipleDamageFromFull()
        {
            int maxHp = 5;
            int hp = 5;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxHp, hp, Vector2.One);

            testHealthBar.Update(0, 3);
            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.HealthPips[4].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[3].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[2].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[1].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[0].Active, pipsString);
        }

        [TestMethod]
        public void TestGenerateLessThanFullHp()
        {
            int maxHp = 5;
            int hp = 3;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxHp, hp, Vector2.One);

            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.HealthPips[4].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[3].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[2].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[1].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[0].Active, pipsString);
        }

        [TestMethod]
        public void TestDealDamageFromLessThanFull()
        {
            int maxHp = 5;
            int hp = 3;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxHp, hp, Vector2.One);

            testHealthBar.Update(0, 2);
            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.HealthPips[4].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[3].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[2].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[1].Active, pipsString);
            Assert.IsTrue(testHealthBar.HealthPips[0].Active, pipsString);
        }

        [TestMethod]
        public void TestDealDamageMoreThanFullFromFull()
        {
            int maxHp = 5;
            int hp = 5;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxHp, hp, Vector2.One);

            testHealthBar.Update(0, 10);
            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.HealthPips[4].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[3].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[2].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[1].Active, pipsString);
            Assert.IsFalse(testHealthBar.HealthPips[0].Active, pipsString);
        }
    }
}