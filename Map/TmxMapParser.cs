using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Map.Objects;
using SolStandard.Map.Objects.EntityProps;
using SolStandard.Utility;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;
using TiledSharp;

namespace SolStandard.Map
{
    enum Layer
    {
        Terrain = 0,
        Collide = 1,
        Entities = 2,
        Units = 3
    }

    /**
     * TmxMapBuilder
     * Responsible for parsing game maps to a Map object that we can use in the game.
     */
    public class TmxMapParser
    {
        private readonly TmxMap tmxMap;
        private readonly ITexture2D mapSprite;
        private readonly List<ITexture2D> unitSprites;

        private List<MapObject[,]> gameTileLayers;

        public TmxMapParser(TmxMap tmxMap, ITexture2D mapSprite, List<ITexture2D> unitSprites)
        {
            this.tmxMap = tmxMap;
            this.mapSprite = mapSprite;
            this.unitSprites = unitSprites;
        }

        private MapObject[,] ObtainTilesFromLayer(Layer tileLayer)
        {
            MapObject[,] tileGrid = new MapObject[tmxMap.Width, tmxMap.Height];

            int tileCounter = 0;

            for (int row = 0; row < tmxMap.Height; row++)
            {
                for (int col = 0; col < tmxMap.Width; col++)
                {
                    int tileId = tmxMap.Layers[(int) tileLayer].Tiles[tileCounter].Gid;

                    if (tileId != 0)
                    {
                        tileGrid[col, row] = new MapTile(new TileCell(mapSprite, GameDriver.CellSize, tileId),
                            new Vector2(col, row));
                    }

                    tileCounter++;
                }
            }

            return tileGrid;
        }


        private MapObject[,] ObtainEntitiesFromLayer(string objectGroupName)
        {
            MapObject[,] entityGrid = new MapObject[tmxMap.Width, tmxMap.Height];

            //Handle the Entities Layer
            foreach (TmxObject currentObject in tmxMap.ObjectGroups[objectGroupName].Objects)
            {
                for (int row = 0; row < tmxMap.Height; row++)
                {
                    for (int col = 0; col < tmxMap.Width; col++)
                    {
                        //NOTE: For some reason, ObjectLayer objects in Tiled measure Y-axis from the bottom of the tile. Compensate in the calculation here.
                        if ((col * GameDriver.CellSize) == (int) currentObject.X &&
                            (row * GameDriver.CellSize) == ((int) currentObject.Y - GameDriver.CellSize))
                        {
                            List<EntityProp> entityProps = new List<EntityProp>();
                            //TODO Add any appropriate properties to the entityProps list

                            int objectTileId = currentObject.Tile.Gid;
                            if (objectTileId != 0)
                            {
                                TileCell tileCell = new TileCell(mapSprite, GameDriver.CellSize, objectTileId);

                                entityGrid[col, row] = new MapEntity(currentObject.Name, tileCell, entityProps,
                                    new Vector2(col, row));
                            }
                        }
                    }
                }
            }

            return entityGrid;
        }

        private MapObject[,] ObtainUnitsFromLayer(string objectGroupName)
        {
            MapObject[,] entityGrid = new MapObject[tmxMap.Width, tmxMap.Height];

            //Handle the Units Layer
            foreach (TmxObject currentObject in tmxMap.ObjectGroups[objectGroupName].Objects)
            {
                for (int row = 0; row < tmxMap.Height; row++)
                {
                    for (int col = 0; col < tmxMap.Width; col++)
                    {
                        //NOTE: For some reason, ObjectLayer objects in Tiled measure Y-axis from the bottom of the tile. Compensate in the calculation here.
                        if ((col * GameDriver.CellSize) != (int) currentObject.X || (row * GameDriver.CellSize) !=
                            ((int) currentObject.Y - GameDriver.CellSize)) continue;
                        List<EntityProp> entityProps = new List<EntityProp>();
                        //TODO Add any appropriate properties to the entityProps list

                        int objectTileId = currentObject.Tile.Gid;
                        if (objectTileId != 0)
                        {
                            Team unitTeam = ObtainUnitTeam(currentObject.Properties["Team"]);
                            UnitClass unitClass = ObtainUnitClass(currentObject.Properties["Class"]);

                            string unitTeamAndClass = unitTeam.ToString() + unitClass.ToString();
                            ITexture2D unitSprite = FetchUnitGraphic(unitTeamAndClass);

                            AnimatedSprite animatedSprite =
                                new AnimatedSprite(unitSprite, GameDriver.CellSize, 15, true);

                            entityGrid[col, row] = new MapEntity(currentObject.Name, animatedSprite,
                                entityProps, new Vector2(col, row));
                        }
                    }
                }
            }

            return entityGrid;
        }


        private UnitClass ObtainUnitClass(string unitClassName)
        {
            foreach (UnitClass unitClass in Enum.GetValues(typeof(UnitClass)))
            {
                if (unitClassName.Equals(unitClass.ToString()))
                {
                    return unitClass;
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
            return unitSprites.Find(texture => texture.GetTexture2D().Name.Contains(unitName));
        }

        public List<MapObject[,]> LoadMapGrid()
        {
            gameTileLayers = new List<MapObject[,]>
            {
                ObtainTilesFromLayer(Layer.Terrain),
                ObtainTilesFromLayer(Layer.Collide),
                ObtainEntitiesFromLayer("Entities"),
                ObtainUnitsFromLayer("Units")
            };

            return gameTileLayers;
        }
    }
}