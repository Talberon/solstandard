using System.Collections.Generic;
using NUnit.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility.Monogame;
using SolStandardTest.Utility.Monogame;
using TiledSharp;

namespace SolStandardTest.Map
{
    [TestFixture]
    public class TmxMapParserTest
    {
        private TmxMapParser objectUnderTest;
        private List<MapElement[,]> mapGrid;
        private List<UnitEntity> unitsFromMap;
        private List<string> unitTextureNames;
        private string worldTileSetTextureName;
        private string terrainTextureName;

        [SetUp]
        public void Setup()
        {
            const string objectTypesXmlPath = "Resources/objecttypes.xml";
            TmxMap tmxMap = new TmxMap("Resources/TmxParserSample_Neo_01.tmx");
            worldTileSetTextureName = "WorldTileSet";
            terrainTextureName = "Terrain";
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

            objectUnderTest =
                new TmxMapParser(tmxMap, new FakeTexture2D(worldTileSetTextureName),
                    new FakeTexture2D(terrainTextureName),
                    unitSprites, objectTypesXmlPath);
            mapGrid = objectUnderTest.LoadMapGrid();
            unitsFromMap = objectUnderTest.LoadUnits();
        }

        [Test]
        public void TestUnits()
        {
            foreach (MapEntity unit in unitsFromMap)
            {
                MapEntity mapUnit = unit;
                if (mapUnit != null)
                    Assert.IsNotNull(
                        unitTextureNames.Find(texture => texture.Contains(mapUnit.TiledProperties["Class"])));
            }
        }

        [Test]
        public void TestEntities()
        {
            MapEntity[,] entitiesGrid = (MapEntity[,]) mapGrid[(int) Layer.Entities];

            int xCoord;
            int yBreakableObstacles = 2;

            //0,2 Stone
            xCoord = 0;
            Assert.AreEqual("Stone", entitiesGrid[xCoord, yBreakableObstacles].Name);
            Assert.AreEqual("BreakableObstacle", entitiesGrid[xCoord, yBreakableObstacles].Type);
            Assert.AreEqual("1", entitiesGrid[xCoord, yBreakableObstacles].TiledProperties["HP"]);
            Assert.AreEqual("false", entitiesGrid[xCoord, yBreakableObstacles].TiledProperties["canMove"]);
            Assert.AreEqual("false", entitiesGrid[xCoord, yBreakableObstacles].TiledProperties["isBroken"]);


            int yBuffTiles = 3;

            //0,3 Scout Tower
            xCoord = 0;
            Assert.AreEqual("Scout Tower", entitiesGrid[xCoord, yBuffTiles].Name);
            Assert.AreEqual("BuffTile", entitiesGrid[xCoord, yBuffTiles].Type);
            Assert.AreEqual("1", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Modifier"]);
            Assert.AreEqual("ATK", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Stat"]);
            Assert.AreEqual("true", entitiesGrid[xCoord, yBuffTiles].TiledProperties["canMove"]);

            //1,3 Forest
            xCoord = 1;
            Assert.AreEqual("Forest", entitiesGrid[xCoord, yBuffTiles].Name);
            Assert.AreEqual("BuffTile", entitiesGrid[xCoord, yBuffTiles].Type);
            Assert.AreEqual("1", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Modifier"]);
            Assert.AreEqual("DEF", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Stat"]);
            Assert.AreEqual("true", entitiesGrid[xCoord, yBuffTiles].TiledProperties["canMove"]);

            //2,3 Town B
            xCoord = 2;
            Assert.AreEqual("Town B", entitiesGrid[xCoord, yBuffTiles].Name);
            Assert.AreEqual("BuffTile", entitiesGrid[xCoord, yBuffTiles].Type);
            Assert.AreEqual("1", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Modifier"]);
            Assert.AreEqual("DEF", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Stat"]);
            Assert.AreEqual("true", entitiesGrid[xCoord, yBuffTiles].TiledProperties["canMove"]);

            //3,3 Hill
            xCoord = 3;
            Assert.AreEqual("Hill", entitiesGrid[xCoord, yBuffTiles].Name);
            Assert.AreEqual("BuffTile", entitiesGrid[xCoord, yBuffTiles].Type);
            Assert.AreEqual("1", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Modifier"]);
            Assert.AreEqual("ATK", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Stat"]);
            Assert.AreEqual("true", entitiesGrid[xCoord, yBuffTiles].TiledProperties["canMove"]);

            //4,3 Mountain
            xCoord = 4;
            Assert.AreEqual("Mountain", entitiesGrid[xCoord, yBuffTiles].Name);
            Assert.AreEqual("BuffTile", entitiesGrid[xCoord, yBuffTiles].Type);
            Assert.AreEqual("3", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Modifier"]);
            Assert.AreEqual("DEF", entitiesGrid[xCoord, yBuffTiles].TiledProperties["Stat"]);
            Assert.AreEqual("false", entitiesGrid[xCoord, yBuffTiles].TiledProperties["canMove"]);

            int yMovables = 4;

            //0,4 Hori Bridge
            xCoord = 0;
            Assert.AreEqual("Hori Bridge", entitiesGrid[xCoord, yMovables].Name);
            Assert.AreEqual("Movable", entitiesGrid[xCoord, yMovables].Type);
            Assert.AreEqual("true", entitiesGrid[xCoord, yMovables].TiledProperties["canMove"]);

            //1,4 Pond
            xCoord = 1;
            Assert.AreEqual("Pond", entitiesGrid[xCoord, yMovables].Name);
            Assert.AreEqual("Movable", entitiesGrid[xCoord, yMovables].Type);
            Assert.AreEqual("false", entitiesGrid[xCoord, yMovables].TiledProperties["canMove"]);
        }

        [Test]
        public void TestCollideLayer()
        {
            for (int col = 0; col < mapGrid[(int) Layer.Collide].GetLength(0); col++)
            {
                //The last two rows of tiles should all exist
                Assert.IsNull(mapGrid[(int) Layer.Collide][col, 0]);
                Assert.IsNull(mapGrid[(int) Layer.Collide][col, 1]);
                Assert.IsNull(mapGrid[(int) Layer.Collide][col, 2]);
                Assert.AreEqual(typeof(MapTile), mapGrid[(int) Layer.Collide][col, 3].GetType());
                Assert.AreEqual(typeof(MapTile), mapGrid[(int) Layer.Collide][col, 4].GetType());
            }
        }

        [Test]
        public void TestTerrainDecorationLayer()
        {
            for (int col = 0; col < mapGrid[(int) Layer.TerrainDecoration].GetLength(0); col++)
            {
                //The 2nd, 3rd and 4th rows of tiles should all exist
                Assert.IsNull(mapGrid[(int) Layer.TerrainDecoration][col, 0]);
                Assert.AreEqual(typeof(MapTile), mapGrid[(int) Layer.TerrainDecoration][col, 1].GetType());
                Assert.AreEqual(typeof(MapTile), mapGrid[(int) Layer.TerrainDecoration][col, 2].GetType());
                Assert.AreEqual(typeof(MapTile), mapGrid[(int) Layer.TerrainDecoration][col, 3].GetType());
                Assert.IsNull(mapGrid[(int) Layer.TerrainDecoration][col, 4]);
            }
        }

        [Test]
        public void TestTerrainLayer()
        {
            for (int col = 0; col < mapGrid[(int) Layer.Terrain].GetLength(0); col++)
            {
                //The 1st, 2nd, 3rd and 4th rows of tiles should all exist
                Assert.AreEqual(typeof(MapTile), mapGrid[(int) Layer.Terrain][col, 0].GetType());
                Assert.AreEqual(typeof(MapTile), mapGrid[(int) Layer.Terrain][col, 1].GetType());
                Assert.AreEqual(typeof(MapTile), mapGrid[(int) Layer.Terrain][col, 2].GetType());
                Assert.AreEqual(typeof(MapTile), mapGrid[(int) Layer.Terrain][col, 3].GetType());
                Assert.IsNull(mapGrid[(int) Layer.Terrain][col, 4]);
            }
        }
    }
}