using NUnit.Framework;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Network;

namespace SolStandardTest.Utility.Network
{
    [TestFixture]
    public class NetworkControlParserTest
    {
        [Test]
        public void testNetworkInput_InvalidBlank()
        {
            Assert.AreEqual(Input.None, NetworkControlParser.ReadNetworkInput(""));
        }

        [Test]
        public void testNetworkInput_InvalidJunk()
        {
            Assert.AreEqual(Input.None, NetworkControlParser.ReadNetworkInput("AIsuhflkasjgh342"));
        }

        [Test]
        public void testNetworkInput_Up()
        {
            Assert.AreEqual(Input.CursorUp, NetworkControlParser.ReadNetworkInput("UP"));
        }

        [Test]
        public void testNetworkInput_Down()
        {
            Assert.AreEqual(Input.CursorDown, NetworkControlParser.ReadNetworkInput("DOWN"));
        }

        [Test]
        public void testNetworkInput_Left()
        {
            Assert.AreEqual(Input.CursorLeft, NetworkControlParser.ReadNetworkInput("LEFT"));
        }

        [Test]
        public void testNetworkInput_Right()
        {
            Assert.AreEqual(Input.CursorRight, NetworkControlParser.ReadNetworkInput("RIGHT"));
        }


        [Test]
        public void testNetworkInput_RsUp()
        {
            Assert.AreEqual(Input.CameraUp, NetworkControlParser.ReadNetworkInput("RS_UP"));
        }

        [Test]
        public void testNetworkInput_RsDown()
        {
            Assert.AreEqual(Input.CameraDown, NetworkControlParser.ReadNetworkInput("RS_DOWN"));
        }

        [Test]
        public void testNetworkInput_RsLeft()
        {
            Assert.AreEqual(Input.CameraLeft, NetworkControlParser.ReadNetworkInput("RS_LEFT"));
        }

        [Test]
        public void testNetworkInput_RsRight()
        {
            Assert.AreEqual(Input.CameraRight, NetworkControlParser.ReadNetworkInput("RS_RIGHT"));
        }


        [Test]
        public void testNetworkInput_A()
        {
            Assert.AreEqual(Input.Confirm, NetworkControlParser.ReadNetworkInput("A"));
        }

        [Test]
        public void testNetworkInput_B()
        {
            Assert.AreEqual(Input.Cancel, NetworkControlParser.ReadNetworkInput("B"));
        }

        [Test]
        public void testNetworkInput_X()
        {
            Assert.AreEqual(Input.ResetToUnit, NetworkControlParser.ReadNetworkInput("X"));
        }

        [Test]
        public void testNetworkInput_Y()
        {
            Assert.AreEqual(Input.CenterCamera, NetworkControlParser.ReadNetworkInput("Y"));
        }


        [Test]
        public void testNetworkInput_LeftBumper()
        {
            Assert.AreEqual(Input.LeftBumper, NetworkControlParser.ReadNetworkInput("LB"));
        }

        [Test]
        public void testNetworkInput_RightBumper()
        {
            Assert.AreEqual(Input.RightBumper, NetworkControlParser.ReadNetworkInput("RB"));
        }

        [Test]
        public void testNetworkInput_LeftTrigger()
        {
            Assert.AreEqual(Input.LeftTrigger, NetworkControlParser.ReadNetworkInput("LT"));
        }

        [Test]
        public void testNetworkInput_RightTrigger()
        {
            Assert.AreEqual(Input.RightTrigger, NetworkControlParser.ReadNetworkInput("RT"));
        }
    }
}