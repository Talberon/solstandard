using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario.Objectives;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Components.LevelSelect
{
    public class MapSelectContext
    {
        public readonly MapSelectHUD MapSelectHUD;
        public readonly MapContainer MapContainer;
        private readonly List<SelectMapEntity> mapSelectEntities;
        private SelectMapEntity currentMapEntity;

        public MapSelectContext(MapSelectHUD mapSelectHUD, MapContainer mapContainer)
        {
            MapSelectHUD = mapSelectHUD;
            MapContainer = mapContainer;
            MapSelectHUD.UpdateTeamSelectWindow();
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

            MapSelectHUD.UpdateMapInfoWindow(CursorAtMapSelectFeature(cursorSlice)
                ? cursorSlice.TerrainEntity.TerrainInfo
                : null);
        }

        public bool CanPressConfirm => MapContainer.GetMapSliceAtCursor().TerrainEntity != null;

        public void SelectMap()
        {
            AssetManager.MenuConfirmSFX.Play();
            MapSlice cursorSlice = MapContainer.GetMapSliceAtCursor();

            if (!CursorAtMapSelectFeature(cursorSlice)) return;
            if (cursorSlice.TerrainEntity.GetType() != typeof(SelectMapEntity)) return;

            var selectMapEntity = (SelectMapEntity) cursorSlice.TerrainEntity;

            if (MapObjectives.IsMultiplayerGame(selectMapEntity.MapObjectives.Scenario))
            {
                Team firstTurn = (GameDriver.Random.Next(2) == 0) ? Team.Blue : Team.Red;

                if (selectMapEntity.Draft)
                {
                    GlobalContext.LoadMapAndScenario(
                        selectMapEntity.MapInfo.FileName,
                        selectMapEntity.MapObjectives.Scenario,
                        firstTurn
                    );

                    if (!CreepPreferences.Instance.CreepsCanSpawn)
                    {
                        foreach (GameUnit creep in GlobalContext.Units.Where(unit => unit.Team == Team.Creep))
                        {
                            creep.KillUnit();
                        }
                    }

                    GlobalContext.DraftContext.StartNewDraft(
                        selectMapEntity.MaxBlueUnits,
                        selectMapEntity.MaxRedUnits,
                        selectMapEntity.MaxDuplicateUnits,
                        firstTurn,
                        selectMapEntity.MapObjectives.Scenario
                    );

                    GlobalContext.CurrentGameState = GlobalContext.GameState.ArmyDraft;
                }
                else
                {
                    StartNonDraftMap(selectMapEntity, firstTurn);
                }
            }
            else
            {
                Team firstTurn = selectMapEntity.SoloTeam;

                if (selectMapEntity.Draft)
                {
                    GlobalContext.LoadMapAndScenario(
                        selectMapEntity.MapInfo.FileName,
                        selectMapEntity.MapObjectives.Scenario,
                        firstTurn
                    );

                    GlobalContext.DraftContext.StartNewSoloDraft(
                        firstTurn == Team.Blue
                            ? selectMapEntity.MaxBlueUnits
                            : selectMapEntity.MaxRedUnits,
                        selectMapEntity.MaxDuplicateUnits,
                        selectMapEntity.SoloTeam,
                        selectMapEntity.MapObjectives.Scenario
                    );

                    GlobalContext.CurrentGameState = GlobalContext.GameState.ArmyDraft;
                }
                else
                {
                    StartNonDraftMap(selectMapEntity, firstTurn);
                }
            }

            GlobalContext.CenterCursorAndCamera();
            PlayMapSong(selectMapEntity);
        }

        private static void StartNonDraftMap(SelectMapEntity selectMapEntity, Team firstTurn)
        {
            GameDriver.NewGame(
                selectMapEntity.MapInfo.FileName,
                selectMapEntity.MapObjectives.Scenario,
                firstTurn
            );
            AssetManager.MenuConfirmSFX.Play();
            PlayMapSong(selectMapEntity);
        }

        public void MoveCursorToNextMap()
        {
            int currentItemIndex = mapSelectEntities.IndexOf(currentMapEntity);

            currentMapEntity = currentItemIndex == mapSelectEntities.Count - 1
                ? mapSelectEntities[0]
                : mapSelectEntities[currentItemIndex + 1];

            GlobalContext.MapCursor.SnapCameraAndCursorToCoordinates(currentMapEntity.MapCoordinates);
            HoverOverEntity();
            AssetManager.MapUnitCancelSFX.Play();
        }

        public void MoveCursorToPreviousMap()
        {
            int currentItemIndex = mapSelectEntities.IndexOf(currentMapEntity);

            currentMapEntity = currentItemIndex == 0
                ? mapSelectEntities.Last()
                : mapSelectEntities[currentItemIndex - 1];

            GlobalContext.MapCursor.SnapCameraAndCursorToCoordinates(currentMapEntity.MapCoordinates);
            HoverOverEntity();
            AssetManager.MapUnitCancelSFX.Play();
        }

        private static void PlayMapSong(SelectMapEntity mapEntity)
        {
            IPlayableAudio songToPlay =
                AssetManager.MusicTracks.Find(song => song.Name.Contains(mapEntity.MapSongName));
            MusicBox.PlayLoop(songToPlay);
        }

        private static bool CursorAtMapSelectFeature(MapSlice cursorSlice)
        {
            return cursorSlice.TerrainEntity != null &&
                   cursorSlice.TerrainEntity.Type == EntityTypes.SelectMap.ToString();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}