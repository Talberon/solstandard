using NUnit.Framework;
using SolStandard.Entity.Unit.Actions.Lancer;

namespace SolStandardTest.Entity.Unit.Actions.Lancer
{
    [TestFixture]
    public class ExecuteTest
    {
        [Test]
        public void FiftyPercentOdd()
        {
            const int attackValue = 7;
            const int percentage = 50;

            int result = Execute.ApplyPercentageRoundedUp(attackValue, percentage);

            Assert.AreEqual(4, result);
        }

        [Test]
        public void FiftyPercentEven()
        {
            const int attackValue = 10;
            const int percentage = 50;

            int result = Execute.ApplyPercentageRoundedUp(attackValue, percentage);

            Assert.AreEqual(5, result);
        }

        [Test]
        public void OneThird()
        {
            const int attackValue = 7;
            const int percentage = 33;

            int result = Execute.ApplyPercentageRoundedUp(attackValue, percentage);

            Assert.AreEqual(3, result);
        }

        [Test]
        public void SixBySixty()
        {
            const int attackValue = 6;
            const int percentage = 60;

            int result = Execute.ApplyPercentageRoundedUp(attackValue, percentage);

            Assert.AreEqual(4, result);
        }

        [Test]
        public void ZeroAttack()
        {
            const int attackValue = 0;
            const int percentage = 60;

            int result = Execute.ApplyPercentageRoundedUp(attackValue, percentage);

            Assert.AreEqual(0, result);
        }

        [Test]
        public void ZeroPercent()
        {
            const int attackValue = 20;
            const int percentage = 0;

            int result = Execute.ApplyPercentageRoundedUp(attackValue, percentage);

            Assert.AreEqual(0, result);
        }
    }
}