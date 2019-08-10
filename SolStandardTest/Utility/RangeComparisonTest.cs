using Microsoft.Xna.Framework;
using NUnit.Framework;
using SolStandard.Utility;

namespace SolStandardTest.Utility
{
    [TestFixture]
    public class RangeComparisonTest
    {
        [Test]
        public void TargetIsSelf_RangeZero()
        {
            Vector2 origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0};
            Vector2 target = origin;

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsTrue(targetIsInRange);
        }

        [Test]
        public void TargetIsSelf_OutOfRange()
        {
            Vector2 origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0};
            Vector2 target = new Vector2(5, 6);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsFalse(targetIsInRange);
        }

        [Test]
        public void TargetWithinAreaOfEffect_North()
        {
            Vector2 origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0, 1, 2};
            Vector2 target = new Vector2(5, 3);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsTrue(targetIsInRange);
        }

        [Test]
        public void TargetWithinAreaOfEffect_SouthWest()
        {
            Vector2 origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0, 1};
            Vector2 target = new Vector2(4, 6);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsTrue(targetIsInRange);
        }

        [Test]
        public void TargetBeyondAreaOfEffect_East()
        {
            Vector2 origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0, 1};
            Vector2 target = new Vector2(8, 5);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsFalse(targetIsInRange);
        }
    }
}