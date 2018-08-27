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

            testHealthBar.DealDamage(1);
            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.Pips[4].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[3].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[2].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[1].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[0].Active, pipsString);
        }
        [TestMethod]
        public void TestDealMultipleDamageFromFull()
        {
            int maxHp = 5;
            int hp = 5;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxHp, hp, Vector2.One);

            testHealthBar.DealDamage(3);
            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.Pips[4].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[3].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[2].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[1].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[0].Active, pipsString);
        }
        
        [TestMethod]
        public void TestGenerateLessThanFullHp()
        {
            int maxHp = 5;
            int hp = 3;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxHp, hp, Vector2.One);

            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.Pips[4].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[3].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[2].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[1].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[0].Active, pipsString);
        }
        
        [TestMethod]
        public void TestDealDamageFromLessThanFull()
        {
            int maxHp = 5;
            int hp = 3;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxHp, hp, Vector2.One);

            testHealthBar.DealDamage(2);
            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.Pips[4].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[3].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[2].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[1].Active, pipsString);
            Assert.IsTrue(testHealthBar.Pips[0].Active, pipsString);
        }
        
        [TestMethod]
        public void TestDealDamageMoreThanFullFromFull()
        {
            int maxHp = 5;
            int hp = 5;
            FakeHealthBar testHealthBar = new FakeHealthBar(maxHp, hp, Vector2.One);

            testHealthBar.DealDamage(10);
            string pipsString = string.Join(",", testHealthBar.PipValues);
            Assert.IsFalse(testHealthBar.Pips[4].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[3].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[2].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[1].Active, pipsString);
            Assert.IsFalse(testHealthBar.Pips[0].Active, pipsString);
        }
    }
}