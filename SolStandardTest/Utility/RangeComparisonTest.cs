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
            var origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0};
            Vector2 target = origin;

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsTrue(targetIsInRange);
        }

        [Test]
        public void TargetIsSelf_OutOfRange()
        {
            var origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0};
            var target = new Vector2(5, 6);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsFalse(targetIsInRange);
        }

        [Test]
        public void TargetWithinAreaOfEffect_North()
        {
            var origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0, 1, 2};
            var target = new Vector2(5, 3);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsTrue(targetIsInRange);
        }

        [Test]
        public void TargetWithinAreaOfEffect_SouthWest()
        {
            var origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0, 1, 2};
            var target = new Vector2(4, 6);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsTrue(targetIsInRange);
        }

        [Test]
        public void TargetBeyondAreaOfEffect_East()
        {
            var origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0, 1};
            var target = new Vector2(8, 5);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsFalse(targetIsInRange);
        }

        [Test]
        public void TargetBeyondAreaOfEffect_NorthEastCorner()
        {
            var origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0, 1, 2};
            var target = new Vector2(7, 7);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsFalse(targetIsInRange);
        }

        [Test]
        public void TargetBeyondAreaOfEffect_NorthEastAlmostCorner()
        {
            var origin = new Vector2(5, 5);
            int[] rangeFromOrigin = {0, 1, 2};
            var target = new Vector2(7, 6);

            bool targetIsInRange = RangeComparison.TargetIsWithinRangeOfOrigin(origin, rangeFromOrigin, target);

            Assert.IsFalse(targetIsInRange);
        }
    }
}