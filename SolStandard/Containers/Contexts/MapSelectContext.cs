using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Containers.View;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts
{
    public class MapSelectContext
    {
        public readonly MapSelectScreenView MapSelectScreenView;
        public readonly MapContainer MapContainer;
        private readonly List<SelectMapEntity> mapSelectEntities;
        private SelectMapEntity currentMapEntity;

        public MapSelectContext(MapSelectScreenView mapSelectScreenView, MapContainer mapContainer)
        {
            MapSelectScreenView = mapSelectScreenView;
            MapContainer = mapContainer;
            MapSelectScreenView.UpdateTeamSelectWindow();
            mapSelectEntities = MapContainer.GetMapEntities().Where(entity => entity is SelectMapEntity)
                .Cast<SelectMapEntity>().ToList();
            currentMapEntity = mapSelectEntities.Last();
        }

        public Vector2 MapCenter =>
            new Vector2(
                (float) Math.Round(MapContainer.MapGridSize.X / 2),
                (float) Math.Round(MapContainer.MapGridSize.Y / 2)
            );

        public void HoverOverEntity()
        {
            MapSlice cursorSlice = MapContainer.GetMapSliceAtCursor();

            MapSelectScreenView.UpdateMapInfoWindow(CursorAtMapSelectFeature(cursorSlice)
                ? cursorSlice.TerrainEntity.TerrainInfo
                : null);
        }

        public bool CanPressConfirm => MapContainer.GetMapSliceAtCursor().TerrainEntity != null;

        public void SelectMap()
        {
            MapSlice cursorSlice = MapContainer.GetMapSliceAtCursor();

            if (!CursorAtMapSelectFeature(cursorSlice)) return;

            if (cursorSlice.TerrainEntity.GetType() != typeof(SelectMapEntity)) return;

            SelectMapEntity selectMapEntity = (SelectMapEntity) cursorSlice.TerrainEntity;

            if (selectMapEntity.Draft)
            {
                AssetManager.MenuConfirmSFX.Play();

                GameContext.LoadMapAndScenario(
                    selectMapEntity.MapInfo.FileName,
                    selectMapEntity.MapObjectives.Scenario
                );

                if (MapObjectives.IsMultiplayerGame(selectMapEntity.MapObjectives.Scenario))
                {
                    GameContext.DraftContext.StartNewDraft(
                        selectMapEntity.MaxBlueUnits,
                        selectMapEntity.MaxRedUnits,
                        selectMapEntity.MaxDuplicateUnits,
                        (GameDriver.Random.Next(2) == 0) ? Team.Blue : Team.Red,
                        selectMapEntity.MapObjectives.Scenario
                    );
                }
                else
                {
                    GameContext.DraftContext.StartNewSoloDraft(
                        selectMapEntity.SoloTeam == Team.Blue
                            ? selectMapEntity.MaxBlueUnits
                            : selectMapEntity.MaxRedUnits,
                        selectMapEntity.MaxDuplicateUnits,
                        selectMapEntity.SoloTeam,
                        selectMapEntity.MapObjectives.Scenario
                    );
                }

                GameContext.CurrentGameState = GameContext.GameState.ArmyDraft;
                GameContext.CenterCursorAndCamera();
                PlayMapSong(selectMapEntity);
            }
            else
            {
                GameDriver.NewGame(selectMapEntity.MapInfo.FileName, selectMapEntity.MapObjectives.Scenario);
                AssetManager.MenuConfirmSFX.Play();
                PlayMapSong(selectMapEntity);
            }
        }

        public void MoveCursorToNextMap()
        {
            int currentItemIndex = mapSelectEntities.IndexOf(currentMapEntity);

            currentMapEntity = currentItemIndex == mapSelectEntities.Count - 1
                ? mapSelectEntities[0]
                : mapSelectEntities[currentItemIndex + 1];

            GameContext.MapCursor.SnapCameraAndCursorToCoordinates(currentMapEntity.MapCoordinates);
            HoverOverEntity();
            AssetManager.MapUnitCancelSFX.Play();
        }

        public void MoveCursorToPreviousMap()
        {
            int currentItemIndex = mapSelectEntities.IndexOf(currentMapEntity);

            currentMapEntity = currentItemIndex == 0
                ? mapSelectEntities.Last()
                : mapSelectEntities[currentItemIndex - 1];

            GameContext.MapCursor.SnapCameraAndCursorToCoordinates(currentMapEntity.MapCoordinates);
            HoverOverEntity();
            AssetManager.MapUnitCancelSFX.Play();
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