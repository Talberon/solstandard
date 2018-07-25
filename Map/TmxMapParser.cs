using Microsoft.Xna.Framework.Graphics;
using SolStandard.Map.Objects;
using SolStandard.Map.Objects.EntityProps;
using SolStandard.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class TmxMapParser
    {
        private const int CELL_SIZE = 64;

        private TmxMap tmxMap;
        private Texture2D mapSprite;

        private List<MapObject[,]> gameTileLayers;

        public TmxMapParser(String tmxFilePath)
        {
            this.tmxMap = new TmxMap(tmxFilePath);

        }

        private MapTile[,] ObtainTilesFromLayer(Layer tileLayer)
        {
            MapTile[,] tileGrid = new MapTile[tmxMap.Width, tmxMap.Height];

            int tileCounter = 0;

            for (int row = 0; row < tmxMap.Height; row++)
            {
                for (int col = 0; col < tmxMap.Width; col++)
                {
                    int tileId = tmxMap.Layers[(int)tileLayer].Tiles[tileCounter].Gid;

                    //public GameTile(Texture2D tileSet, string layer, string name, string type, int tileIndex)
                    if (tileId != 0)
                    {
                        tileGrid[col, row] = new MapTile(new TileCell(mapSprite, CELL_SIZE, tileId));
                    }

                    tileCounter++;
                }
            }

            return tileGrid;
        }


        private MapEntity[,] ObtainEntitiesFromLayer(string objectGroupName)
        {
            MapEntity[,] entityGrid = new MapEntity[tmxMap.Width, tmxMap.Height];

            for (int row = 0; row < tmxMap.Height; row+= CELL_SIZE)
            {
                for (int col = 0; col < tmxMap.Width; col+= CELL_SIZE)
                {
                    //Handle the Entities Layer
                    foreach (TmxObject currentObject in tmxMap.ObjectGroups[objectGroupName].Objects)
                    {
                        //NOTE: For some reason, ObjectLayer objects in Tiled measure Y-axis from the bottom of the tile. Compensate in the calculation here.
                        if ((col == currentObject.X) && (row == currentObject.Y - CELL_SIZE))
                        {
                            List<EntityProp> entityProps = new List<EntityProp>();
                            //TODO Add any appropriate properties to the entityProps list
                            
                            int objectTileId = currentObject.Tile.Gid;
                            if (objectTileId != 0)
                            {
                                //FIXME Pick a value other than 0 here or refactor TileCell to not need it
                                TileCell tileCell = new TileCell(mapSprite, CELL_SIZE, 0);

                                entityGrid[col, row] = new MapEntity(currentObject.Name, tileCell, entityProps);
                            }

                        }
                    }
                }
            }

            return entityGrid;
        }

        public Map LoadMap(String pathToTmxMap)
        {
            gameTileLayers = new List<MapObject[,]>();
            gameTileLayers.Add(ObtainTilesFromLayer(Layer.Terrain));
            gameTileLayers.Add(ObtainTilesFromLayer(Layer.Collide));
            gameTileLayers.Add(ObtainEntitiesFromLayer("Entities"));
            gameTileLayers.Add(ObtainEntitiesFromLayer("Units"));

            //TODO FIXME add Map constructor that takes a set of tileLayers
            return null;
            //return new Map(gameTileLayers);
        }
    }
}
