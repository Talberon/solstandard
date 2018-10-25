using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using SolStandard.Containers.View;
using SolStandard.Entity.General;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts
{
    public class MapSelectContext
    {
        public readonly SelectMapUI SelectMapUI;
        public readonly MapContainer MapContainer;

        public MapSelectContext(SelectMapUI selectMapUI, MapContainer mapContainer)
        {
            SelectMapUI = selectMapUI;
            MapContainer = mapContainer;
            Vector2 mapCenter = new Vector2(
                (float) Math.Round(MapContainer.MapGridSize.X / 2),
                (float) Math.Round(MapContainer.MapGridSize.Y / 2)
            );
            MapContainer.MapCursor.SnapCursorToCoordinates(mapCenter);
        }

        public void HoverOverEntity()
        {
            MapSlice cursorSlice = MapContainer.GetMapSliceAtCursor();

            if (CursorAtMapSelectFeature(cursorSlice))
            {
                SelectMapUI.UpdateMapInfoWindow(cursorSlice.TerrainEntity.TerrainInfo);
            }
            else
            {
                SelectMapUI.UpdateMapInfoWindow(null);
            }
        }

        public void SelectMap()
        {
            MapSlice cursorSlice = MapContainer.GetMapSliceAtCursor();

            if (CursorAtMapSelectFeature(cursorSlice))
            {
                if (cursorSlice.TerrainEntity.GetType() == typeof(SelectMapEntity))
                {
                    SelectMapEntity selectMapEntity = (SelectMapEntity) cursorSlice.TerrainEntity;
                    GameDriver.NewGame(selectMapEntity.MapInfo.FileName, selectMapEntity.MapObjectives.Scenario);
                    AssetManager.MenuConfirmSFX.Play();
                    PlayMapSong(selectMapEntity);
                }
            }
        }

        private static void PlayMapSong(SelectMapEntity mapEntity)
        {
            Song songToPlay = AssetManager.MusicTracks.Find(song => song.Name.Contains(mapEntity.MapSongName));
            MusicBox.PlayLoop(songToPlay, 0.3f);
        }

        private static bool CursorAtMapSelectFeature(MapSlice cursorSlice)
        {
            return cursorSlice.TerrainEntity != null &&
                   cursorSlice.TerrainEntity.Type == EntityTypes.SelectMap.ToString();
        }
    }
}