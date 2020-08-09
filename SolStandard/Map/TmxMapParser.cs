using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Scenario.Objectives;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
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
        Dynamic,
        OverlayEffect
    }

    /**
     * TmxMapBuilder
     * Responsible for converting Tiled maps into map objects in-game.
     */
    public class TmxMapParser
    {
        private readonly string objectTypesDefaultXmlPath;
        private readonly TmxMap tmxMap;
        private readonly ITexture2D worldTileSetSprite;
        private readonly ITexture2D terrainSprite;
        private readonly List<ITexture2D> unitSprites;

        private List<MapElement[,]> gameTileLayers;
        private List<UnitEntity> unitLayer;

        private List<KeyValuePair<ITexture2D, int>> TilesetsSortedByFirstGid { get; }

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
            var tilesetGids = new List<KeyValuePair<ITexture2D, int>>();
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
                new MapElement[tmxMap.Width, tmxMap.Height], //Preview
                new MapElement[tmxMap.Width, tmxMap.Height], //Dynamic
                new MapElement[tmxMap.Width, tmxMap.Height] //OverlayEffect
            };

            return gameTileLayers;
        }

        public List<UnitEntity> LoadUnits()
        {
            unitLayer = new List<UnitEntity>();
            foreach (UnitEntity unit in ObtainUnitsFromLayer("Units"))
            {
                if (unit != null) unitLayer.Add(unit);
            }

            return unitLayer;
        }

        public List<IItem> LoadMapLoot()
        {
            TerrainEntity[,] mapInventoryLayer = ObtainEntitiesFromLayer("Loot");

            List<IItem> loot = mapInventoryLayer.OfType<IItem>().ToList();

            return loot;
        }

        public List<CreepEntity> LoadSummons()
        {
            return ObtainUnitsFromLayer("Summons").Cast<CreepEntity>().Where(unit => unit != null).ToList();
        }

        private MapElement[,] ObtainTilesFromLayer(Layer tileLayer)
        {
            var tileGrid = new MapElement[tmxMap.Width, tmxMap.Height];

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
                                    GameDriver.CellSizeVector,
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
            var entityGrid = new TerrainEntity[tmxMap.Width, tmxMap.Height];

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
                                        GameDriver.CellSizeVector,
                                        FindTileId(objectTileId)
                                    );
                                }
                                else
                                {
                                    tileSprite = GetAnimatedTile(animatedTile, currentObject.Tile);
                                }

                                var tileEntityType =
                                    (EntityTypes) Enum.Parse(typeof(EntityTypes), currentObject.Type);

                                switch (tileEntityType)
                                {
                                    case EntityTypes.BreakableObstacle:
                                        entityGrid[col, row] = new BreakableObstacle(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
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
                                            Convert.ToInt32(currentProperties["atkBonus"]),
                                            Convert.ToInt32(currentProperties["retBonus"]),
                                            Convert.ToInt32(currentProperties["blockBonus"]),
                                            Convert.ToInt32(currentProperties["luckBonus"])
                                        );
                                        break;
                                    case EntityTypes.Chest:
                                        IItem specificChestItem = (currentProperties["item"] != string.Empty)
                                            ? mapLoot.Single(item => item.Name == currentProperties["item"]).Duplicate()
                                            : null;

                                        entityGrid[col, row] = new Chest(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToBoolean(currentProperties["isLocked"]),
                                            Convert.ToBoolean(currentProperties["isOpen"]),
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            currentProperties["range"]
                                                .Split(',').Select(n => Convert.ToInt32(n)).ToArray(),
                                            Convert.ToInt32(currentProperties["gold"]) + GameDriver.Random.Next(0, 5),
                                            specificChestItem,
                                            currentProperties["itemPool"]
                                        );

                                        break;
                                    case EntityTypes.Decoration:
                                        entityGrid[col, row] = new Decoration(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row)
                                        );
                                        break;
                                    case EntityTypes.Door:
                                        entityGrid[col, row] = new Door(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToBoolean(currentProperties["isLocked"]),
                                            Convert.ToBoolean(currentProperties["isOpen"]),
                                            currentProperties["range"]
                                                .Split(',').Select(n => Convert.ToInt32(n)).ToArray(),
                                            Convert.ToInt32(currentProperties["HP"])
                                        );
                                        break;
                                    case EntityTypes.Movable:
                                        entityGrid[col, row] = new Movable(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToBoolean(currentProperties["canMove"])
                                        );
                                        break;
                                    case EntityTypes.Portal:
                                        entityGrid[col, row] = new Portal(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
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
                                            currentProperties["triggersId"]
                                        );
                                        break;
                                    case EntityTypes.Key:
                                        entityGrid[col, row] = new Key(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["usedWith"],
                                            currentProperties["range"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            currentProperties["itemPool"],
                                            Convert.ToBoolean(currentProperties["masterKey"])
                                        );
                                        break;
                                    case EntityTypes.Drawbridge:
                                        entityGrid[col, row] = new Drawbridge(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToBoolean(currentProperties["isOpen"])
                                        );
                                        break;
                                    case EntityTypes.Artillery:
                                        entityGrid[col, row] = new Artillery(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            currentProperties["range"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            Convert.ToInt32(currentProperties["damage"])
                                        );
                                        break;
                                    case EntityTypes.Railgun:
                                        entityGrid[col, row] = new Railgun(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            Convert.ToInt32(currentProperties["range"]),
                                            Convert.ToInt32(currentProperties["damage"])
                                        );
                                        break;
                                    case EntityTypes.SelectMap:
                                        string mapFileName = currentProperties["mapFileName"];
                                        var derivedMapInfo = new MapInfo(currentObject.Name, mapFileName);

                                        entityGrid[col, row] = new SelectMapEntity(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            derivedMapInfo,
                                            currentProperties["mapSong"],
                                            new MapObjectives(
                                                Convert.ToBoolean(currentProperties["modeAssassinate"]),
                                                Convert.ToBoolean(currentProperties["modeRoutArmy"]),
                                                Convert.ToBoolean(currentProperties["modeSeize"]),
                                                Convert.ToBoolean(currentProperties["modeTaxes"]),
                                                Convert.ToInt32(currentProperties["valueTaxes"]),
                                                Convert.ToBoolean(currentProperties["modeSoloDefeatBoss"]),
                                                (Team) Enum.Parse(typeof(Team), currentProperties["modeSolo.team"]),
                                                Convert.ToBoolean(currentProperties["modeEscape"]),
                                                (Team) Enum.Parse(typeof(Team), currentProperties["modeEscape.team"]),
                                                Convert.ToBoolean(currentProperties["modeRelic.vs"]),
                                                Convert.ToBoolean(currentProperties["modeRelic.coop"]),
                                                Convert.ToInt32(currentProperties["modeRelic.goal"])
                                            ),
                                            Convert.ToBoolean(currentProperties["draftUnits"]),
                                            Convert.ToInt32(currentProperties["maxUnitsBlue"]),
                                            Convert.ToInt32(currentProperties["maxUnitsRed"]),
                                            Convert.ToInt32(currentProperties["maxDuplicateUnits"]),
                                            (Team) Enum.Parse(typeof(Team), currentProperties["modeSolo.team"]),
                                            AssetManager.MapPreviewTextures.FirstOrDefault(texture =>
                                                texture.Name.EndsWith("/" + mapFileName.Substring(0,
                                                                          mapFileName.Length - (".tmx").Length))),
                                            Convert.ToInt32(currentProperties["timeEstimateOutOfFive"])
                                        );
                                        break;
                                    case EntityTypes.Seize:
                                        entityGrid[col, row] = new SeizeEntity(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToBoolean(currentProperties["capturableByBlue"]),
                                            Convert.ToBoolean(currentProperties["capturableByRed"])
                                        );
                                        break;
                                    case EntityTypes.Pushable:
                                        entityGrid[col, row] = new PushBlock(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row)
                                        );
                                        break;
                                    case EntityTypes.PressurePlate:
                                        entityGrid[col, row] = new PressurePlate(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
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
                                            Convert.ToBoolean(currentProperties["enabled"]),
                                            Convert.ToBoolean(currentProperties["willSnare"]),
                                            Convert.ToBoolean(currentProperties["willSlow"]),
                                            currentProperties["itemPool"]
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
                                            Convert.ToInt32(currentProperties["usesRemaining"]),
                                            currentProperties["itemPool"]
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
                                            Convert.ToInt32(currentProperties["usesRemaining"]),
                                            currentProperties["itemPool"]
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
                                            Convert.ToInt32(currentProperties["hpHealed"]),
                                            currentProperties["itemPool"]
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
                                                .ToArray(),
                                            currentProperties["itemPool"]
                                        );
                                        break;
                                    case EntityTypes.Barricade:
                                        entityGrid[col, row] = new Barricade(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToInt32(currentProperties["HP"]),
                                            currentProperties["itemPool"]
                                        );
                                        break;
                                    case EntityTypes.Deployment:
                                        entityGrid[col, row] = new DeployTile(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            (Team) Enum.Parse(typeof(Team), currentProperties["Team"])
                                        );
                                        break;
                                    case EntityTypes.CreepDeploy:
                                        entityGrid[col, row] = new CreepDeployTile(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["creepPool"],
                                            Convert.ToBoolean(currentProperties["copyCreep"])
                                        );
                                        break;
                                    case EntityTypes.Bank:
                                        entityGrid[col, row] = new Bank(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            currentProperties["interactRange"].Split(',')
                                                .Select(n => Convert.ToInt32(n))
                                                .ToArray()
                                        );
                                        break;
                                    case EntityTypes.Vendor:
                                        string[] itemNames = currentProperties["items"].Split('|');

                                        List<IItem> vendorItems = mapLoot
                                            .Where(item => itemNames.Contains(item.Name))
                                            .OrderBy(item => Array.IndexOf(itemNames, item.Name))
                                            .ToList();

                                        entityGrid[col, row] = new Vendor(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToBoolean(currentProperties["canMove"]),
                                            currentProperties["interactRange"].Split(',')
                                                .Select(n => Convert.ToInt32(n)).ToArray(),
                                            //Items
                                            vendorItems,
                                            //Prices
                                            currentProperties["prices"].Split('|')
                                                .Select(n => Convert.ToInt32(n)).ToArray(),
                                            //Quantities
                                            currentProperties["quantities"].Split('|')
                                                .Select(n => Convert.ToInt32(n)).ToArray()
                                        );
                                        break;
                                    case EntityTypes.RecoveryTile:
                                        entityGrid[col, row] = new RecoveryTile(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            Convert.ToInt32(currentProperties["amrPerTurn"]),
                                            Convert.ToInt32(currentProperties["hpPerTurn"])
                                        );
                                        break;
                                    case EntityTypes.LadderBridge:
                                        entityGrid[col, row] = new LadderBridge(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["itemPool"],
                                            Convert.ToBoolean(currentProperties["canMove"])
                                        );
                                        break;
                                    case EntityTypes.Magnet:
                                        entityGrid[col, row] = new Magnet(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["pickupRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            currentProperties["range"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            Convert.ToInt32(currentProperties["usesRemaining"]),
                                            currentProperties["itemPool"]
                                        );
                                        break;
                                    case EntityTypes.Bomb:
                                        entityGrid[col, row] = new Bomb(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["range"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            Convert.ToInt32(currentProperties["damage"]),
                                            Convert.ToInt32(currentProperties["turnsRemaining"]),
                                            currentProperties["itemPool"]
                                        );
                                        break;
                                    case EntityTypes.Contract:
                                        entityGrid[col, row] = new Contract(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["pickupRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            currentProperties["itemPool"],
                                            Convert.ToBoolean(currentProperties["forSpecificUnit"]),
                                            (currentProperties["specificRole"] != string.Empty)
                                                ? (Role) Enum.Parse(typeof(Role), currentProperties["specificRole"])
                                                : Role.Silhouette
                                        );
                                        break;
                                    case EntityTypes.RecallCharm:
                                        entityGrid[col, row] = new RecallCharm(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["recallId"],
                                            currentProperties["pickupRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            currentProperties["itemPool"],
                                            currentProperties["deployRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            Convert.ToInt32(currentProperties["usesRemaining"])
                                        );
                                        break;
                                    case EntityTypes.Escape:
                                        entityGrid[col, row] = new EscapeEntity(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            (currentProperties["team"].Equals(Team.Blue.ToString())),
                                            (currentProperties["team"].Equals(Team.Red.ToString()))
                                        );
                                        break;
                                    case EntityTypes.Relic:
                                        entityGrid[col, row] = new Relic(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            currentProperties["pickupRange"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray(),
                                            currentProperties["itemPool"]
                                        );
                                        break;
                                    case EntityTypes.Piston:
                                        entityGrid[col, row] = new Piston(currentObject.Name, currentObject.Type,
                                            currentProperties["direction"], new Vector2(col, row));
                                        break;
                                    case EntityTypes.Launchpad:
                                        entityGrid[col, row] = new Launchpad(
                                            currentObject.Name,
                                            currentObject.Type,
                                            new Vector2(col, row),
                                            currentProperties["radius"].Split(',').Select(n => Convert.ToInt32(n))
                                                .ToArray()
                                        );
                                        break;
                                    case EntityTypes.SpringTrap:
                                        entityGrid[col, row] = new SpringTrap(
                                            currentObject.Name,
                                            currentObject.Type,
                                            new Vector2(col, row),
                                            new Vector2(
                                                Convert.ToInt32(currentProperties["trigger.landingCoordinates.x"]),
                                                Convert.ToInt32(currentProperties["trigger.landingCoordinates.y"])
                                            )
                                        );
                                        break;
                                    case EntityTypes.Crossing:
                                        entityGrid[col, row] = new Crossing(
                                            currentObject.Name,
                                            currentObject.Type,
                                            tileSprite,
                                            new Vector2(col, row),
                                            (Direction) Enum.Parse(typeof(Direction), currentProperties["direction"],
                                                true)
                                        );
                                        break;
                                    default:
                                        throw new IndexOutOfRangeException(
                                            $"Entity type {currentObject.Type} does not exist!"
                                        );
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
            var unitGrid = new UnitEntity[tmxMap.Width, tmxMap.Height];

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
                        if (objectTileId == 0) continue;

                        Dictionary<string, string> currentProperties = GetDefaultPropertiesAndOverrides(currentObject);
                        Team unitTeam = ObtainUnitTeam(currentProperties["Team"]);
                        Role role = ObtainUnitClass(currentProperties["Class"]);
                        bool isCommander = Convert.ToBoolean(currentProperties["Commander"]);
                        string itemsList = (currentProperties.ContainsKey("Items"))
                            ? currentProperties["Items"]
                            : string.Empty;
                        string[] unitInventory = (itemsList == string.Empty) ? new string[0] : itemsList.Split('|');

                        unitGrid[col, row] = UnitGenerator.GenerateMapEntity(currentObject.Name,
                            currentObject.Type, role, unitTeam, isCommander, unitInventory, unitSprites,
                            new Vector2(col, row), currentProperties);
                    }
                }
            }

            return unitGrid;
        }

        private Dictionary<string, string> GetDefaultPropertiesAndOverrides(TmxObject tmxObject)
        {
            var combinedProperties = new Dictionary<string, string>(tmxObject.Properties);

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
            List<int> tileIds = animatedTile.AnimationFrames.Select(tmxAnimationFrame => tmxAnimationFrame.Id).ToList();

            //Hold the id values for each frame

            var tileSprite = new AnimatedTileSprite(
                FindTileSet(tile.Gid),
                tileIds,
                GameDriver.CellSizeVector
            );

            return tileSprite;
        }
    }
}