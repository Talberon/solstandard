using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Entity.Unit;
using SolStandard.Logic;
using SolStandard.Map.Objects.Cursor;

namespace SolStandard.Rules
{
    public static class MapCursorHover
    {
        //FIXME TODO Figure out some way to reduce the number of necessary parameters here
        public static void Hover(MapScene mapScene, MapSlice hoverTiles, List<GameUnit> units, MapStaticHud mapStaticHud)
        {
            GameUnit selectedMapUnit = UnitSelector.SelectUnit(units, hoverTiles.UnitEntity);

            //FirstUnit Window
            mapScene.LeftUnitPortraitWindow = mapStaticHud.GenerateUnitPortraitWindow(selectedMapUnit);
            mapScene.LeftUnitDetailWindow = mapStaticHud.GenerateUnitDetailWindow(selectedMapUnit);

            //SecondUnit Window
            mapScene.RightUnitPortraitWindow = mapStaticHud.GenerateUnitPortraitWindow(selectedMapUnit);
            mapScene.RightUnitDetailWindow = mapStaticHud.GenerateUnitDetailWindow(selectedMapUnit);

            //Terrain (Entity) Window
            mapScene.TerrainEntityWindow = mapStaticHud.GenerateTerrainWindow(hoverTiles.GeneralEntity);
        }
    }
}