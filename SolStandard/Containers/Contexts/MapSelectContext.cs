using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Containers.View;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts
{
    public class MapSelectContext
    {
        public readonly MapSelectScreenView MapSelectScreenView;
        public readonly MapContainer MapContainer;

        public MapSelectContext(MapSelectScreenView mapSelectScreenView, MapContainer mapContainer)
        {
            MapSelectScreenView = mapSelectScreenView;
            MapContainer = mapContainer;
            MapSelectScreenView.UpdateTeamSelectWindow();
        }

        public Vector2 MapCenter
        {
            get
            {
                return new Vector2(
                    (float) Math.Round(MapContainer.MapGridSize.X / 2),
                    (float) Math.Round(MapContainer.MapGridSize.Y / 2)
                );
            }
        }

        public void HoverOverEntity()
        {
            MapSlice cursorSlice = MapContainer.GetMapSliceAtCursor();

            if (CursorAtMapSelectFeature(cursorSlice))
            {
                MapSelectScreenView.UpdateMapInfoWindow(cursorSlice.TerrainEntity.TerrainInfo);
            }
            else
            {
                MapSelectScreenView.UpdateMapInfoWindow(null);
            }
        }

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