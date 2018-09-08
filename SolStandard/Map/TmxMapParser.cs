using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
        Terrain = 0,
        Collide = 1,
        Entities = 2,
        Dynamic = 3
    }

    /**
     * TmxMapBuilder
     * Responsible for parsing game maps to a Map object that we can use in the game.
     */
    public class TmxMapParser
    {
        private readonly string objectTypesDefaultXmlPath;
        private readonly TmxMap tmxMap;
        private readonly ITexture2D mapSprite;
        private readonly List<ITexture2D> unitSprites;

        private List<MapElement[,]> gameTileLayers;
        private List<UnitEntity> unitLayer;

        public TmxMapParser(TmxMap tmxMap, ITexture2D mapSprite, List<ITexture2D> unitSprites,
            string objectTypesDefaultXmlPath)
        {
            this.tmxMap = tmxMap;
            this.mapSprite = mapSprite;
            this.unitSprites = unitSprites;
            this.objectTypesDefaultXmlPath = objectTypesDefaultXmlPath;
        }

        public List<MapElement[,]> LoadMapGrid()
        {
            gameTileLayers = new List<MapElement[,]>
            {
                ObtainTilesFromLayer(Layer.Terrain),
                ObtainTilesFromLayer(Layer.Collide),
                // ReSharper disable once CoVariantArrayConversion
                ObtainEntitiesFromLayer("Entities"),
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

        private MapElement[,] ObtainTilesFromLayer(Layer tileLayer)
        {
            MapElement[,] tileGrid = new MapElement[tmxMap.Width, tmxMap.Height];

            int tileCounter = 0;

            for (int row = 0; row < tmxMap.Height; row++)
            {
                for (int col = 0; col < tmxMap.Width; col++)
                {
                    int tileId = tmxMap.Layers[(int) tileLayer].Tiles[tileCounter].Gid;

                    if (tileId != 0)
                    {
                        tileGrid[col, row] = new MapTile(new SpriteAtlas(mapSprite, new Vector2(GameDriver.CellSize), tileId),
                            new Vector2(col, row));
                    }

                    tileCounter++;
                }
            }

            return tileGrid;
        }


        private MapEntity[,] ObtainEntitiesFromLayer(string objectGroupName)
        {
            MapEntity[,] entityGrid = new MapEntity[tmxMap.Width, tmxMap.Height];

            //Handle the Entities Layer
            foreach (TmxObject currentObject in tmxMap.ObjectGroups[objectGroupName].Objects)
            {
                for (int col = 0; col < tmxMap.Width; col++)
                {
                    for (int row = 0; row < tmxMap.Height; row++)
                    {
                        //NOTE: For some reason, ObjectLayer objects in Tiled measure Y-axis from the bottom of the tile.c Compensate in the calculation here.
                        if ((col * GameDriver.CellSize) == (int) currentObject.X &&
                            (row * GameDriver.CellSize) == ((int) currentObject.Y - GameDriver.CellSize))
                        {
                            int objectTileId = currentObject.Tile.Gid;
                            if (objectTileId != 0)
                            {
                                Dictionary<string, string> currentProperties =
                                    GetDefaultPropertiesAndOverrides(currentObject);
                                SpriteAtlas spriteAtlas = new SpriteAtlas(mapSprite, new Vector2(GameDriver.CellSize), objectTileId);

                                entityGrid[col, row] = new MapEntity(currentObject.Name, currentObject.Type, spriteAtlas,
                                    new Vector2(col, row), currentProperties);
                            }
                        }
                    }
                }
            }

            return entityGrid;
        }

        private UnitEntity[,] ObtainUnitsFromLayer(string objectGroupName)
        {
            UnitEntity[,] entityGrid = new UnitEntity[tmxMap.Width, tmxMap.Height];

            //Handle the Units Layer
            foreach (TmxObject currentObject in tmxMap.ObjectGroups[objectGroupName].Objects)
            {
                for (int col = 0; col < tmxMap.Width; col++)
                {
                    for (int row = 0; row < tmxMap.Height; row++)
                    {
                        //NOTE: For some reason, ObjectLayer objects in Tiled measure Y-axis from the bottom of the tile. Compensate in the calculation here.
                        if ((col * GameDriver.CellSize) != (int) currentObject.X || (row * GameDriver.CellSize) !=
                            ((int) currentObject.Y - GameDriver.CellSize)) continue;

                        int objectTileId = currentObject.Tile.Gid;
                        if (objectTileId != 0)
                        {
                            Dictionary<string, string> currentProperties =
                                GetDefaultPropertiesAndOverrides(currentObject);
                            Team unitTeam = ObtainUnitTeam(currentProperties["Team"]);
                            Role role = ObtainUnitClass(currentProperties["Class"]);

                            string unitTeamAndClass = unitTeam.ToString() + role.ToString();
                            ITexture2D unitSprite = FetchUnitGraphic(unitTeamAndClass);

                            UnitSprite animatedSprite = new UnitSprite(unitSprite, GameDriver.CellSize, 15, false);

                            entityGrid[col, row] = new UnitEntity(currentObject.Name, currentObject.Type, animatedSprite,
                                new Vector2(col, row), currentProperties);
                        }
                    }
                }
            }

            return entityGrid;
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

        private Role ObtainUnitClass(string unitClassName)
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

        private Team ObtainUnitTeam(string unitTeamName)
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

        private ITexture2D FetchUnitGraphic(string unitName)
        {
            return unitSprites.Find(texture => texture.Name.Contains(unitName));
        }
    }
}