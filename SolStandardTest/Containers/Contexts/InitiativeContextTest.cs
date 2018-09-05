using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;

namespace SolStandardTest.Containers.Contexts
{
    [TestClass]
    public class InitiativeContextTest
    {
        private static readonly GameUnit BlueUnit =
            new GameUnit("BlueGuy", Team.Blue, UnitClass.Monarch, null,
                new UnitStatistics(10, 2, 2, 1, 1, 2, new[] {1}, 0), null, null, null);

        private static readonly GameUnit RedUnit =
            new GameUnit("RedGuy", Team.Red, UnitClass.Monarch, null,
                new UnitStatistics(10, 2, 2, 1, 1, 2, new[] {1}, 0), null, null, null);

        [TestInitialize]
        public void SetupBeforeEachTest()
        {
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v3_BlueFirst()
        {
            List<GameUnit> unitList = GenerateUnitList(3, 3);

            InitiativeContext initiativeContext = new InitiativeContext(unitList, Team.Blue);

            Assert.AreEqual(BlueUnit.UnitTeam, initiativeContext.InitiativeList[0].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[1].UnitTeam);
            Assert.AreEqual(BlueUnit.UnitTeam, initiativeContext.InitiativeList[2].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[3].UnitTeam);
            Assert.AreEqual(BlueUnit.UnitTeam, initiativeContext.InitiativeList[4].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[5].UnitTeam);
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v3_RedFirst()
        {
            List<GameUnit> unitList = GenerateUnitList(3, 3);

            InitiativeContext initiativeContext = new InitiativeContext(unitList, Team.Red);

            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[0].UnitTeam);
            Assert.AreEqual(BlueUnit.UnitTeam, initiativeContext.InitiativeList[1].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[2].UnitTeam);
            Assert.AreEqual(BlueUnit.UnitTeam, initiativeContext.InitiativeList[3].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[4].UnitTeam);
            Assert.AreEqual(BlueUnit.UnitTeam, initiativeContext.InitiativeList[5].UnitTeam);
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v5()
        {
            List<GameUnit> unitList = GenerateUnitList(3, 5);

            InitiativeContext initiativeContext = new InitiativeContext(unitList, Team.Blue);

            Assert.AreEqual(BlueUnit.UnitTeam, initiativeContext.InitiativeList[0].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[1].UnitTeam);
            Assert.AreEqual(BlueUnit.UnitTeam, initiativeContext.InitiativeList[2].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[3].UnitTeam);
            Assert.AreEqual(BlueUnit.UnitTeam, initiativeContext.InitiativeList[4].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[5].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[6].UnitTeam);
            Assert.AreEqual(RedUnit.UnitTeam, initiativeContext.InitiativeList[7].UnitTeam);
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v8()
        {
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v11()
        {
        }


        private static List<GameUnit> GenerateUnitList(int redUnits, int blueUnits)
        {
            List<GameUnit> unitList = new List<GameUnit>();

            for (int i = 0; i < redUnits; i++)
            {
                unitList.Add(RedUnit);
            }

            for (int i = 0; i < blueUnits; i++)
            {
                unitList.Add(BlueUnit);
            }

            return unitList;
        }
    }
}