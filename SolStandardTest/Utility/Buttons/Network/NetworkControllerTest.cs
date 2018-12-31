using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.Network;

namespace SolStandardTest.Utility.Buttons.Network
{
    [TestFixture]
    public class NetworkControllerTest
    {
        [Test]
        public void TestSerialization()
        {
            NetworkController controller = new NetworkController(PlayerIndex.One);

            controller.Press(Input.Confirm);
            controller.Press(Input.CursorUp);
            controller.Release(Input.CursorDown);

            IFormatter formatter = new BinaryFormatter();

            //TODO Fix this path or write to stream instead of file
            const string fileName =
                "C:/Users/heroc/git/solstandard/SolStandardTest/Resources/Output/NetworkSerializerTest.txt";

            FileStream writeStream = new FileStream(fileName, FileMode.Create);
            formatter.Serialize(writeStream, controller);
            writeStream.Close();

            FileStream readStream = new FileStream(fileName, FileMode.Open);
            NetworkController readController = (NetworkController) formatter.Deserialize(readStream);

            Console.WriteLine(readController);

            Assert.IsTrue(readController.Confirm.Pressed);
            Assert.IsTrue(readController.CursorUp.Pressed);
            Assert.IsTrue(readController.CursorDown.Released);

            Assert.IsFalse(readController.Cancel.Pressed);
        }
    }
}