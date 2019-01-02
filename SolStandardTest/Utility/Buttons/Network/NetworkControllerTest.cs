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

            NetworkController readController;

            using (Stream writeStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(writeStream, controller);

                Console.WriteLine("Stream length: " + writeStream.Length + Environment.NewLine);

                writeStream.Seek(0, SeekOrigin.Begin);
                readController = (NetworkController) formatter.Deserialize(writeStream);

                writeStream.Close();
            }

            Console.WriteLine(readController);

            Assert.IsTrue(readController.Confirm.Pressed);
            Assert.IsTrue(readController.CursorUp.Pressed);
            Assert.IsTrue(readController.CursorDown.Released);

            Assert.IsFalse(readController.Cancel.Pressed);
        }
    }
}