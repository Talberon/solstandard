using System.Collections.Generic;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Logic;
using SolStandard.Map.Objects.Cursor;

namespace SolStandard.Rules
{
    public static class MapCursorHover
    {
        //FIXME TODO Figure out some way to reduce the number of necessary parameters here
        public static void Hover(MapUI mapUi, MapSlice hoverTiles, List<GameUnit> units, MapStaticHud mapStaticHud)
        {
            GameUnit selectedMapUnit = UnitSelector.SelectUnit(units, hoverTiles.UnitEntity);

            //FirstUnit Window
            mapUi.LeftUnitPortraitWindow = mapStaticHud.GenerateUnitPortraitWindow(selectedMapUnit);
            mapUi.LeftUnitDetailWindow = mapStaticHud.GenerateUnitDetailWindow(selectedMapUnit);

            //SecondUnit Window
            mapUi.RightUnitPortraitWindow = mapStaticHud.GenerateUnitPortraitWindow(selectedMapUnit);
            mapUi.RightUnitDetailWindow = mapStaticHud.GenerateUnitDetailWindow(selectedMapUnit);

            //Terrain (Entity) Window
            mapUi.TerrainEntityWindow = mapStaticHud.GenerateTerrainWindow(hoverTiles.GeneralEntity);
        }
    }
}