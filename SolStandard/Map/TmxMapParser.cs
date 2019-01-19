using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;
using SolStandard.Utility.Parsing;
using TiledSharp;

namespace SolStandard.Map
{
    public enum Layer
    {
        Terrain,
        TerrainDecoration,
        Collide,
        Overlay,
        Entities,
        Items,
        Preview,
        Dynamic
    }

    /**
     * TmxMapBuilder
     * Responsible for parsing game maps to a SelectMapEntity object that we can use in the game.
     */
    public class TmxMapParser
    {
        private static readonly Dictionary<string, EntityTypes> EntityDictionary = new Dictionary<string, EntityTypes>
        {
            {"BreakableObstacle", EntityTypes.BreakableObstacle},
            {"BuffTile", EntityTypes.BuffTile},
            {"Chest", EntityTypes.Chest},
            {"Decoration", EntityTypes.Decoration},
            {"Door", EntityTypes.Door},
            {"Drawbridge", EntityTypes.Drawbridge},
            {"Movable", EntityTypes.Movable},
            {"SelectMap", EntityTypes.SelectMap},
            {"Unit", EntityTypes.Unit},
            {"Portal", EntityTypes.Portal},
            {"Switch", EntityTypes.Switch},
            {"Currency", EntityTypes.Currency},
            {"Key", EntityTypes.Key},
            {"Artillery", EntityTypes.Artillery},
            {"Railgun", EntityTypes.Railgun},
            {"Seize", EntityTypes.Seize},
            {"Pushable", EntityTypes.Pushable},
            {"PressurePlate", EntityTypes.PressurePlate},
            {"Trap", EntityTypes.Trap},
            {"Creep", EntityTypes.Creep},
            {"Weapon", EntityTypes.Weapon},
            {"Blink", EntityTypes.Blink},
            {"HP Potion", EntityTypes.HealthPotion},
            {"BuffItem", EntityTypes.BuffItem},
            {"Barricade", EntityTypes.Barricade}
        };

        private readonly string objectTypesDefaultXmlPath;
        private readonly TmxMap tmxMap;
        private readonly ITexture2D worldTileSetSprite;
        private readonly ITexture2D terrainSprite;
        private readonly List<ITexture2D> unitSprites;

        private List<MapElement[,]> gameTileLayers;
        private List<UnitEntity> unitLayer;

        private List<KeyValuePair<ITexture2D, int>> TilesetsSortedByFirstGid { get; set; }

        public TmxMapParser(TmxMap tmxMap, ITexture2D worldTileSetSprite, ITexture2D terrainSprite,
            List<ITexture2D> unitSprites, string objectTypesDefaultXmlPath)
        {
            this.tmxMap = tmxMap;
            this.worldTileSetSprite = worldTileSetSprite;
            this.unitSprites = unitSprites;
            this.objectTypesDefaultXmlPath = objectTypesDefaultXmlPath;
            this.terrainSprite = terrainSprite;
            TilesetsSortedByFirstGid = SortTilesetsByFirstGid();
        }

        private List<KeyValuePair<ITexture2D, int>> SortTilesetsByFirstGid()
        {
            List<KeyValuePair<ITexture2D, int>> tilesetGids = new List<KeyValuePair<ITexture2D, int>>();
            foreach (TmxTileset tileset in tmxMap.Tilesets)
            {
                if (worldTileSetSprite.Name.Contains(tileset.Name))
                {
                    tilesetGids.Add(new KeyValuePair<ITexture2D, int>(worldTileSetSprite, tileset.FirstGid));
                }

                if (terrainSprite.Name.Contains(tileset.Name))
                {
                    tilesetGids.Add(new KeyValuePair<ITexture2D, int>(terrainSprite, tileset.FirstGid));
                }
            }

            tilesetGids.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            return tilesetGids;
        }

        private ITexture2D FindTileSet(int gid)
        {
            ITexture2D tileSet = null;

            foreach (KeyValuePair<ITexture2D, int> item in TilesetsSortedByFirstGid)
            {
                if (gid >= item.Value)
                {
                    tileSet = item.Key;
                }
            }

            return tileSet;
        }

        private int FindTileId(int gid)
        {
            int nextFirstGid = 1;

            foreach (KeyValuePair<ITexture2D, int> item in TilesetsSortedByFirstGid)
            {
                if (gid >= item.Value)
                {
                    nextFirstGid = item.Value;
                }
            }

            return gid - nextFirstGid;
        }

        public List<MapElement[,]> LoadMapGrid()
        {
            List<IItem> mapLoot = LoadMapLoot();

            gameTileLayers = new List<MapElement[,]>
            {
                ObtainTilesFromLayer(Layer.Terrain),
                ObtainTilesFromLayer(Layer.TerrainDecoration),
                ObtainTilesFromLayer(Layer.Collide),
                ObtainTilesFromLayer(Layer.Overlay),
                // ReSharper disable once CoVariantArrayConversion
                ObtainEntitiesFromLayer("Entities", mapLoot),
                // ReSharper disable once CoVariantArrayConversion
                ObtainEntitiesFromLayer("Items"),
                new MapElement[tmxMap.Width, tmxMap.Height],
                new MapElement[tmxMap.Width, tmxMap.Height]
            };

            return gameTileLayers;
        }

        public List<UnitEntity> LoadUnits()
        {
            unitLayer = new List<UnitEntity>();
            foreach (UnitEntity unit in ObtainUnitsFromLayer("Units"))
            {
                unitLayer.Add(unit);
            }

            return unitLayer;
        }

        public List<IItem> LoadMapLoot()
        {
            TerrainEntity[,] mapInventoryLayer = ObtainEntitiesFromLayer("Loot");

            List<IItem> loot = mapInventoryLayer.OfType<IItem>().ToList();

            return loot;
        }

        private MapElement[,] ObtainTilesFromLayer(Layer tileLayer)
        {
            MapElement[,] tileGrid = new MapElement[tmxMap.Width, tmxMap.Height];

            int tileCounter = 0;

            for (int row = 0; row < tmxMap.Height; row++)
            {
                for (int col = 0; col < tmxMap.Width; col++)
                {
                    TmxLayerTile tile = tmxMap.Layers[(int) tileLayer].Tiles[tileCounter];

                    if (tile.Gid != 0)
                    {
                        TmxTilesetTile animatedTile = TilesetTile(tile);

                        if (animatedTile == null)
                        {
                            tileGrid[col, row] = new MapTile(
                                new SpriteAtlas(
                                    FindTileSet(tile.Gid),
                                    new Vector2(GameDriver.CellSize),
                                    FindTileId(tile.Gid)
                                ),
                                new Vector2(col, row));
                        }
                        else
                        {
                            tileGrid[col, row] = new MapTile(GetAnimatedTile(animatedTile, tile),
                                new Vector2(col, row));
                        }
                    }

                    tileCounter++;
                }
            }

            return tileGrid;
        }

        private TerrainEntity[,] ObtainEntitiesFromLayer(string objectGroupName)
        {
            return ObtainEntitiesFromLayer(objectGroupName, new List<IItem>());
        }

        private TerrainEntity[,] ObtainEntitiesFromLayer(string objectGroupName, List<IItem> mapLoot)
        {
            TerrainEntity[,] entityGrid = new TerrainEntity[tmxMap.Width, tmxMap.Height];

            //Handle the Entities Layer
            foreach (TmxObject currentObject in tmxMap.ObjectGroups[objectGroupName].Objects)
            {
                for (int col = 0; col < tmxMap.Width; col++)
                {
                    for (int row = 0; row < tmxMap.Height; row++)
                    {
                        //NOTE: For some reason, ObjectLayer objects in Tiled measure Y-axis from the bottom of the tile.
                        //Compensate in the calculation here.
                        if ((col * GameDriver.CellSize) == (int) currentObject.X &&
                            (row * GameDriver.CellSize) == ((int) currentObject.Y - GameDriver.CellSize))
                        {
                            int objectTileId = currentObject.Tile.Gid;
                            if (objectTileId != 0)
                            {
                                Dictionary<string, string> currentProperties =
                                    GetDefaultPropertiesAndOverrides(currentObject);

                                IRenderable tileSprite;

                                TmxTilesetTile animatedTile = TilesetTile(currentObject.Tile);

                                if (animatedTile == null)
                                {
                                    tileSprite = new SpriteAtlas(
                                        FindTileSet(objectTileId),
                                        new Vector2(GameDriver.CellSize),
                                        FindTileId(objectTileId)
                                    );
                                }
                                else
                                {
                                    tileSprite = GetAnimatedTile(animatedTile, currentObject.Tile);
                                }

                                EntityTypes tileEntityType = EntityDictionary[currentObject.Type];

                                switch (tileEntityType)
                                {
                                    case EntityTypes.BreakableObstacle:
                                        entityGrid[col, row] = new BreakableObstacle(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToInt32(currentProperties["HP"]),
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            Convert.ToBoolean(currentProperties["isBroken"]),
                                            Convert.ToInt32(currentProperties["gold"]),
                                            (currentProperties["item"] != string.Empty)
                                                ? mapLoot.Single(item => item.Name == currentProperties["item"])
                                                    .Duplicate()
                                                : null
                                        );
                                        break;
                                    case EntityTypes.BuffTile:
                                        entityGrid[col, row] = new BuffTile(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToInt32(currentProperties["Modifier"]),
                                            currentProperties["Stat"],
                                            Convert.ToBoolean(currentProperties["canMove"])
                                        );
                                        break;
                                    case EntityTypes.Chest:
                                        entityGrid[col, row] = new Chest(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToBoolean(currentProperties["isLocked"]),
                                            Convert.ToBoolean(currentProperties["isOpen"]),
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            currentProperties["range"]
                                                .Split(',').Select(n => Convert.ToInt32(n)).ToArray(),
                                            Convert.ToInt32(currentProperties["gold"]) + GameDriver.Random.Next(0, 5),
                                            (currentProperties["item"] != string.Empty)
                                                ? mapLoot.Single(item => item.Name == currentProperties["item"])
                                                    .Duplicate()
                                                : null
                                        );
                                        break;
                                    case EntityTypes.Decoration:
                                        entityGrid[col, row] = new Decoration(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties
                                        );
                                        break;
                                    case EntityTypes.Door:
                                        entityGrid[col, row] = new Door(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToBoolean(currentProperties["isLocked"]),
                                            Convert.ToBoolean(currentProperties["isOpen"]),
                                            currentProperties["range"]
                                                .Split(',').Select(n => Convert.ToInt32(n)).ToArray(),
                                            Convert.ToBoolean(currentProperties["canMove"])
                                        );
                                        break;
                                    case EntityTypes.Movable:
                                        entityGrid[col, row] = new Movable(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToBoolean(currentProperties["canMove"])
                                        );
                                        break;
                                    case EntityTypes.Portal:
                                        entityGrid[col, row] = new Portal(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            currentProperties["destinationId"],
                                            currentProperties["range"]
                                                .Split(',').Select(n => Convert.ToInt32(n)).ToArray()
                                        );
                                        break;
                                    case EntityTypes.Currency:
                                        entityGrid[col, row] = new Currency(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToInt32(currentProperties["value"]),
                                            currentProperties["range"]
                                                .Split(',').Select(n => Convert.ToInt32(n)).ToArray()
                                        );
                                        break;
                                    case EntityTypes.Switch:
                                        entityGrid[col, row] = new Switch(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            currentProperties["triggersId"]
                                        );
                                        break;
                                    case EntityTypes.Key:
                                        entityGrid[col, row] = new Key(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            currentProperties["usedWith"],
                                            currentProperties["range"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray()
                                        );
                                        break;
                                    case EntityTypes.Drawbridge:
                                        entityGrid[col, row] = new Drawbridge(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToBoolean(currentProperties["isOpen"])
                                        );
                                        break;
                                    case EntityTypes.Artillery:
                                        entityGrid[col, row] = new Artillery(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            currentProperties["range"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray()
                                        );
                                        break;
                                    case EntityTypes.Railgun:
                                        entityGrid[col, row] = new Railgun(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            Convert.ToInt32(currentProperties["range"])
                                        );
                                        break;
                                    case EntityTypes.SelectMap:
                                        MapInfo derivedMapInfo = new MapInfo(currentObject.Name,
                                            currentProperties["mapFileName"]);

                                        entityGrid[col, row] = new SelectMapEntity(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            derivedMapInfo,
                                            currentProperties["mapSong"],
                                            new MapObjectives(
                                                Convert.ToBoolean(currentProperties["modeAssassinate"]),
                                                Convert.ToBoolean(currentProperties["modeRoutArmy"]),
                                                Convert.ToBoolean(currentProperties["modeSeize"]),
                                                Convert.ToBoolean(currentProperties["modeTaxes"]),
                                                Convert.ToInt32(currentProperties["valueTaxes"])
                                            )
                                        );
                                        break;
                                    case EntityTypes.Seize:
                                        entityGrid[col, row] = new SeizeEntity(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            Convert.ToBoolean(currentProperties["capturableByBlue"]),
                                            Convert.ToBoolean(currentProperties["capturableByRed"])
                                        );
                                        break;
                                    case EntityTypes.Pushable:
                                        entityGrid[col, row] = new PushBlock(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties
                                        );
                                        break;
                                    case EntityTypes.PressurePlate:
                                        entityGrid[col, row] = new PressurePlate(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties,
                                            currentProperties["triggersId"],
                                            Convert.ToBoolean(currentProperties["triggerOnRelease"])
                                        );
                                        break;
                                    case EntityTypes.Trap:
                                        entityGrid[col, row] = new TrapEntity(
                                            currentObject.Name,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToInt32(currentProperties["damage"]),
                                            Convert.ToInt32(currentProperties["triggersRemaining"]),
                                            Convert.ToBoolean(currentProperties["limitedTriggers"]),
                                            Convert.ToBoolean(currentProperties["enabled"])
                                        );
                                        break;
                                    case EntityTypes.Weapon:
                                        entityGrid[col, row] = new Weapon(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["pickupRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            Convert.ToInt32(currentProperties["atkValue"]),
                                            Convert.ToInt32(currentProperties["luckModifier"]),
                                            currentProperties["atkRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            Convert.ToInt32(currentProperties["usesRemaining"])
                                        );
                                        break;
                                    case EntityTypes.Blink:
                                        entityGrid[col, row] = new BlinkItem(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["pickupRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            currentProperties["blinkRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            Convert.ToInt32(currentProperties["usesRemaining"])
                                        );
                                        break;
                                    case EntityTypes.HealthPotion:
                                        entityGrid[col, row] = new HealthPotion(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["pickupRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            Convert.ToInt32(currentProperties["hpHealed"])
                                        );
                                        break;
                                    case EntityTypes.BuffItem:
                                        entityGrid[col, row] = new BuffItem(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["stat"],
                                            Convert.ToInt32(currentProperties["modifier"]),
                                            Convert.ToInt32(currentProperties["duration"]),
                                        currentProperties["pickupRange"].Split(',').Select(n => Convert.ToInt32(n))
                                            .ToArray()
                                        );
                                        break;
                                    case EntityTypes.Barricade:
                                        entityGrid[col, row] = new Barricade(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToInt32(currentProperties["HP"])
                                        );
                                        break;
                                    default:
                                        entityGrid[col, row] = new TerrainEntity(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties
                                        );
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return entityGrid;
        }

        private UnitEntity[,] ObtainUnitsFromLayer(string objectGroupName)
        {
            UnitEntity[,] unitGrid = new UnitEntity[tmxMap.Width, tmxMap.Height];

            //Handle the Units Layer
            foreach (TmxObject currentObject in tmxMap.ObjectGroups[objectGroupName].Objects)
            {
                for (int col = 0; col < tmxMap.Width; col++)
                {
                    for (int row = 0; row < tmxMap.Height; row++)
                    {
                        //NOTE: For some reason, ObjectLayer objects in Tiled measure Y-axis from the bottom of the tile.
                        //Compensate in the calculation here.
                        if ((col * GameDriver.CellSize) != (int) currentObject.X || (row * GameDriver.CellSize) !=
                            ((int) currentObject.Y - GameDriver.CellSize)) continue;

                        int objectTileId = currentObject.Tile.Gid;
                        if (objectTileId != 0)
                        {
                            Dictionary<string, string> currentProperties =
                                GetDefaultPropertiesAndOverrides(currentObject);
                            Team unitTeam = ObtainUnitTeam(currentProperties["Team"]);
                            Role role = ObtainUnitClass(currentProperties["Class"]);

                            ITexture2D unitSprite = FetchUnitGraphic(unitTeam.ToString(), role.ToString());

                            Vector2 unitScale = new Vector2(unitSprite.Width) / 2.5f;
                            const int unitAnimationFrames = 4;
                            const int unitAnimationDelay = 12;

                            UnitSpriteSheet animatedSpriteSheet = new UnitSpriteSheet(
                                unitSprite,
                                unitSprite.Width / unitAnimationFrames,
                                unitScale,
                                unitAnimationDelay,
                                false,
                                Color.White
                            );

                            unitGrid[col, row] = new UnitEntity(currentObject.Name, currentObject.Type,
                                animatedSpriteSheet, new Vector2(col, row), currentProperties);
                        }
                    }
                }
            }

            return unitGrid;
        }


        private Dictionary<string, string> GetDefaultPropertiesAndOverrides(TmxObject tmxObject)
        {
            Dictionary<string, string> combinedProperties = new Dictionary<string, string>(tmxObject.Properties);

            foreach (KeyValuePair<string, string> property in GetDefaultPropertiesForType(tmxObject.Type))
            {
                if (!tmxObject.Properties.ContainsKey(property.Key))
                {
                    combinedProperties.Add(property.Key, property.Value);
                }
            }

            return combinedProperties;
        }

        private Dictionary<string, string> GetDefaultPropertiesForType(string parameterObjectType)
        {
            Dictionary<string, Dictionary<string, string>> objectTypesInFile =
                ObjectTypesXmlParser.ParseObjectTypesXml(objectTypesDefaultXmlPath);

            return objectTypesInFile[parameterObjectType];
        }

        private static Role ObtainUnitClass(string unitClassName)
        {
            foreach (Role unitRole in Enum.GetValues(typeof(Role)))
            {
                if (unitClassName.Equals(unitRole.ToString()))
                {
                    return unitRole;
                }
            }

            throw new UnitClassNotFoundException();
        }

        private static Team ObtainUnitTeam(string unitTeamName)
        {
            foreach (Team unitTeam in Enum.GetValues(typeof(Team)))
            {
                if (unitTeamName.Equals(unitTeam.ToString()))
                {
                    return unitTeam;
                }
            }

            throw new TeamNotFoundException();
        }


        private TmxTilesetTile TilesetTile(TmxLayerTile tile)
        {
            //Check if tileset has animation associated with it.
            string tilesetPath = FindTileSet(tile.Gid).Name;
            string tilesetName = tilesetPath.Substring(tilesetPath.LastIndexOf('/') + 1);

            TmxTilesetTile animatedTile = tmxMap.Tilesets[tilesetName].Tiles
                .SingleOrDefault(
                    tilesetTile =>
                        tilesetTile.Id == FindTileId(tile.Gid) && tilesetTile.AnimationFrames.Count > 0
                );
            return animatedTile;
        }

        private AnimatedTileSprite GetAnimatedTile(TmxTilesetTile animatedTile, TmxLayerTile tile)
        {
            List<int> tileIds = new List<int>();

            //Hold the id values for each frame
            foreach (TmxAnimationFrame tmxAnimationFrame in animatedTile.AnimationFrames)
            {
                tileIds.Add(tmxAnimationFrame.Id);
            }

            AnimatedTileSprite tileSprite = new AnimatedTileSprite(
                FindTileSet(tile.Gid),
                tileIds,
                new Vector2(GameDriver.CellSize)
            );

            return tileSprite;
        }


        private ITexture2D FetchUnitGraphic(string unitTeam, string role)
        {
            string unitTeamAndClass = unitTeam + role;
            return unitSprites.Find(texture => texture.Name.Contains(unitTeamAndClass));
        }
    }
}