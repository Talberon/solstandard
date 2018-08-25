using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility.Monogame;
using SolStandardTest.Utility.Monogame;
using TiledSharp;

namespace SolStandardTest.Map
{
    [TestClass]
    public class TmxMapParserTest
    {
        private TmxMapParser objectUnderTest;
        private List<MapElement[,]> mapGrid;
        private List<string> unitTextureNames;
        private string tileTextureName;

        [TestInitialize]
        public void Setup()
        {
            const string objectTypesXmlPath = "Resources/objecttypes.xml";
            TmxMap tmxMap = new TmxMap("Resources/TmxParserSample_01.tmx");
            tileTextureName = "Tiles";
            unitTextureNames = new List<string>
            {
                "RedMage",
                "RedArcher",
                "RedChampion",
                "RedMonarch",
                "BlueMage",
                "BlueArcher",
                "BlueChampion",
                "BlueMonarch"
            };

            List<ITexture2D> unitSprites = new List<ITexture2D>();
            foreach (string textureName in unitTextureNames)
            {
                unitSprites.Add(new FakeTexture2D(textureName));
            }

            objectUnderTest = new TmxMapParser(tmxMap, new FakeTexture2D(tileTextureName), unitSprites, objectTypesXmlPath);
            mapGrid = objectUnderTest.LoadMapGrid();
        }

        [TestMethod]
        public void TestUnits()
        {
            MapElement[,] unitGrid = mapGrid[(int) Layer.Units];

            foreach (MapElement o in unitGrid)
            {
                MapEntity mapUnit = (MapEntity) o;
                if (mapUnit != null)
                    Assert.IsNotNull(unitTextureNames.Find(texture => texture.Contains(mapUnit.Name)));
            }
        }

        [TestMethod]
        public void TestEntities()
        {
            MapEntity[,] entitiesGrid = (MapEntity[,]) mapGrid[(int) Layer.Entities];

            int xCoord;
            int yCoord = 2;
            
            //0,2 Trees
            xCoord = 0;
            Assert.AreEqual("Trees", entitiesGrid[xCoord,yCoord].Name);
            Assert.AreEqual("BuffTile", entitiesGrid[xCoord,yCoord].Type);
            Assert.AreEqual("DEF", entitiesGrid[xCoord,yCoord].TiledProperties["Stat"]);
            Assert.AreEqual("1", entitiesGrid[xCoord,yCoord].TiledProperties["Modifier"]);
            Assert.AreEqual("true", entitiesGrid[xCoord,yCoord].TiledProperties["canMove"]); //Default value; needs to be picked up from objecttypes.xml
            
            //1,2 Tower
            xCoord = 1;
            Assert.AreEqual("Tower", entitiesGrid[xCoord,yCoord].Name);
            Assert.AreEqual("BuffTile", entitiesGrid[xCoord,yCoord].Type);
            Assert.AreEqual("DEF", entitiesGrid[xCoord,yCoord].TiledProperties["Stat"]);
            Assert.AreEqual("2", entitiesGrid[xCoord,yCoord].TiledProperties["Modifier"]);
            Assert.AreEqual("true", entitiesGrid[xCoord,yCoord].TiledProperties["canMove"]);

            //5,2 Breakable Tree
            xCoord = 5;
            Assert.AreEqual("Tree", entitiesGrid[xCoord,yCoord].Name);
            Assert.AreEqual("BreakableObstacle", entitiesGrid[xCoord,yCoord].Type);
            Assert.AreEqual("1", entitiesGrid[xCoord,yCoord].TiledProperties["HP"]);
            Assert.AreEqual("false", entitiesGrid[xCoord,yCoord].TiledProperties["isBroken"]);
            Assert.AreEqual("false", entitiesGrid[xCoord,yCoord].TiledProperties["canMove"]);

            //6,2 Bridge
            xCoord = 6;
            Assert.AreEqual("Bridge", entitiesGrid[xCoord,yCoord].Name);
            Assert.AreEqual("Movable", entitiesGrid[xCoord,yCoord].Type);
            Assert.AreEqual("true", entitiesGrid[xCoord,yCoord].TiledProperties["canMove"]);
            
            //12,2 Moss Decoration
            xCoord = 12;
            Assert.AreEqual("Moss", entitiesGrid[xCoord,yCoord].Name);
            Assert.AreEqual("Decoration", entitiesGrid[xCoord,yCoord].Type);

        }

        [TestMethod]
        public void TestCollideLayer()
        {
            
        }

        [TestMethod]
        public void TestTerrainLayer()
        {
        }

    }
}