using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandardTest.Utility.Monogame;

namespace SolStandardTest.Containers.Contexts
{
    [TestClass]
    public class InitiativeContextTest
    {
        private static readonly GameUnit BlueUnit =
            new GameUnit("BlueGuy", Team.Blue, Role.Monarch, null, new UnitStatistics(10, 2, 2, 2, 2, 2, new[] {1}),
                new FakeTexture2D(""), new FakeTexture2D(""), new FakeTexture2D(""), new List<UnitAction>());

        private static readonly GameUnit RedUnit =
            new GameUnit("RedGuy", Team.Red, Role.Monarch, null, new UnitStatistics(10, 2, 2, 2, 2, 2, new[] {1}),
                new FakeTexture2D(""), new FakeTexture2D(""), new FakeTexture2D(""), new List<UnitAction>());

        [TestMethod]
        public void testInitiativeListRandomizer_3v3_BlueFirst()
        {
            List<GameUnit> unitList = GenerateUnitList(3, 3);

            InitiativeContext initiativeContext = new InitiativeContext(unitList, Team.Blue);

            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[0].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[1].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[2].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[3].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[4].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[5].Team);
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v3_RedFirst()
        {
            List<GameUnit> unitList = GenerateUnitList(3, 3);

            InitiativeContext initiativeContext = new InitiativeContext(unitList, Team.Red);

            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[0].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[1].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[2].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[3].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[4].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[5].Team);
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v5_BlueMajority()
        {
            List<GameUnit> unitList = GenerateUnitList(redUnits: 3, blueUnits: 5);

            InitiativeContext initiativeContext = new InitiativeContext(unitList, Team.Blue);

            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[0].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[1].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[2].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[3].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[4].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[5].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[6].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[7].Team);
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v5_RedMajority()
        {
            List<GameUnit> unitList = GenerateUnitList(redUnits: 5, blueUnits: 3);

            InitiativeContext initiativeContext = new InitiativeContext(unitList, Team.Blue);

            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[0].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[1].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[2].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[3].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[4].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[5].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[6].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[7].Team);
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v8()
        {
            List<GameUnit> unitList = GenerateUnitList(redUnits: 3, blueUnits: 8);

            InitiativeContext initiativeContext = new InitiativeContext(unitList, Team.Blue);

            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[0].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[1].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[2].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[3].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[4].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[5].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[6].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[7].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[8].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[9].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[10].Team);
        }

        [TestMethod]
        public void testInitiativeListRandomizer_3v11()
        {
            List<GameUnit> unitList = GenerateUnitList(redUnits: 11, blueUnits: 3);

            InitiativeContext initiativeContext = new InitiativeContext(unitList, Team.Blue);

            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[0].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[1].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[2].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[3].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[4].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[5].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[6].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[7].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[8].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[9].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[10].Team);
            Assert.AreEqual(BlueUnit.Team, initiativeContext.InitiativeList[11].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[12].Team);
            Assert.AreEqual(RedUnit.Team, initiativeContext.InitiativeList[13].Team);
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