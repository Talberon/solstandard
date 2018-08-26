using SolStandard.Containers.Contexts;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Logic;
using SolStandard.Map.Elements.Cursor;

namespace SolStandard.Rules
{
    public static class MapCursorHover
    {
        public static void Hover(MapContext.TurnState turnState, MapUI mapUi, MapSlice hoverTiles,
            MapHudGenerator mapHudGenerator)
        {
            GameUnit hoverMapUnit = UnitSelector.SelectUnit(hoverTiles.UnitEntity);


            if (turnState != MapContext.TurnState.SelectUnit)
            {
                //SecondUnit Window
                mapUi.RightUnitPortraitWindow = mapHudGenerator.GenerateUnitPortraitWindow(hoverMapUnit);
                mapUi.RightUnitDetailWindow = mapHudGenerator.GenerateUnitDetailWindow(hoverMapUnit);
            }
            else
            {
                //FirstUnit Window
                mapUi.LeftUnitPortraitWindow = mapHudGenerator.GenerateUnitPortraitWindow(hoverMapUnit);
                mapUi.LeftUnitDetailWindow = mapHudGenerator.GenerateUnitDetailWindow(hoverMapUnit);

                //SecondUnit Window
                mapUi.RightUnitPortraitWindow = null;
                mapUi.RightUnitDetailWindow = null;
            }

            //Terrain (Entity) Window
            mapUi.TerrainEntityWindow = mapHudGenerator.GenerateTerrainWindow(hoverTiles.GeneralEntity);
        }
    }
}