using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Utility;
using SolStandardTest.Utility.Monogame;

namespace SolStandardTest.Containers.Contexts
{
    [TestClass]
    public class MapContextTest
    {
        //TODO test CreateMoveGrid
        [TestMethod]
        public void CreateMoveGridTest()
        {
            Vector2 exampleOrigin = new Vector2(10f,15f);
            int exampleMaximumDistance = 5;
            //MapLayer realMapLayer = new MapLayer();//TODO Figure out how I'm going to test a layer of the map
            TextureCell fakeTextureCell = new TextureCell(new FakeTexture2D("FakeTexture"), 1,0);


        }
        
    }
}