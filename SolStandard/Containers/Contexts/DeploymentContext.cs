using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts
{
    public class DeploymentContext
    {
        public DeploymentView DeploymentView { get; private set; }
        private readonly List<GameUnit> blueArmy;
        private readonly List<GameUnit> redArmy;
        private readonly MapContainer map;
        private GameUnit currentUnit;
        public Team CurrentTurn { get; private set; }

        public DeploymentContext(List<GameUnit> blueArmy, List<GameUnit> redArmy, MapContainer map, Team firstTurn)
        {
            this.blueArmy = blueArmy;
            this.redArmy = redArmy;
            this.map = map;
            CurrentTurn = firstTurn;
            currentUnit = GetArmy(CurrentTurn).First();
            DeploymentView = new DeploymentView(blueArmy, redArmy, currentUnit, GameContext.Scenario);
            MoveToNextDeploymentTile();
        }

        public void SelectNextUnit()
        {
            AssetManager.MapUnitCancelSFX.Play();

            List<GameUnit> activeArmy = GetArmy(CurrentTurn);
            int currentUnitIndex = activeArmy.IndexOf(currentUnit);

            int nextIndex = (currentUnitIndex + 1 > activeArmy.Count - 1) ? 0 : currentUnitIndex + 1;

            currentUnit = activeArmy[nextIndex];
            DeploymentView.UpdateRosterLists(blueArmy, redArmy, currentUnit);
        }

        public void SelectPreviousUnit()
        {
            AssetManager.MapUnitCancelSFX.Play();

            List<GameUnit> activeArmy = GetArmy(CurrentTurn);
            int currentUnitIndex = activeArmy.IndexOf(currentUnit);

            int nextIndex = (currentUnitIndex - 1 < 0) ? activeArmy.Count - 1 : currentUnitIndex - 1;

            currentUnit = activeArmy[nextIndex];
            DeploymentView.UpdateRosterLists(blueArmy, redArmy, currentUnit);
        }

        public void TryDeployUnit()
        {
            if (TargetTileIsValidDeploymentTile)
            {
                AssetManager.MapUnitCancelSFX.Play();
                PlaceUnitInTile();
                PassTurn();
                DeploymentView.UpdateRosterLists(blueArmy, redArmy, currentUnit);
                MoveToNextDeploymentTile();
            }
            else
            {
                AssetManager.WarningSFX.Play();
                map.AddNewToastAtMapCursor("Place a unit at an unoccupied deployment tile for your team!", 50);
            }
        }

        private void PlaceUnitInTile()
        {
            if (MapContainer.GetMapSliceAtCoordinates(map.MapCursor.MapCoordinates).TerrainEntity is DeployTile)
            {
                MapContainer.GameGrid[(int) Layer.Entities]
                    [(int) map.MapCursor.MapCoordinates.X, (int) map.MapCursor.MapCoordinates.Y] = null;

                currentUnit.UnitEntity.MapCoordinates = map.MapCursor.MapCoordinates;
            }

            GameContext.Units.Add(currentUnit);
            GetArmy(currentUnit.Team).Remove(currentUnit);
            DeploymentView.UpdateRosterLists(blueArmy, redArmy, currentUnit);
        }

        public void MoveCursorOnMap(Direction direction)
        {
            map.MapCursor.MoveCursorInDirection(direction);
            UpdateHoverView();
        }

        private void UpdateHoverView()
        {
            MapSlice hoverSlice = GameContext.GameMapContext.MapContainer.GetMapSliceAtCursor();
            GameUnit hoverUnit = UnitSelector.SelectUnit(hoverSlice.UnitEntity);

            MapContainer.ClearDynamicAndPreviewGrids();

            if (hoverUnit != null)
            {
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                    .GenerateThreatGrid(hoverSlice.MapCoordinates, hoverUnit, hoverUnit.Team);
            }

            DeploymentView.UpdateHoverUnitWindows(hoverUnit);
            UpdateItemPreview(hoverSlice);

            DeploymentView.SetEntityWindow(hoverSlice);
        }

        private void UpdateItemPreview(MapSlice hoverSlice)
        {
            List<IItem> items = GameMapContext.CollectItemsFromSlice(hoverSlice);

            Color windowColor = GameMapView.ItemTerrainWindowColor;

            GameUnit hoverUnit = UnitSelector.SelectUnit(hoverSlice.UnitEntity);
            if (hoverUnit != null)
            {
                windowColor = TeamUtility.DetermineTeamColor(hoverUnit.Team);
            }

            if (items.Count > 0)
            {
                DeploymentView.GenerateItemDetailWindow(items, windowColor);
            }
            else
            {
                DeploymentView.CloseItemDetailWindow();
            }
        }

        public void MoveToNextDeploymentTile()
        {
            List<MapEntity> mapEntities = MapContainer.GetMapEntities();
            List<MapEntity> deployTiles = mapEntities.Where(tile => tile is DeployTile).ToList();
            if (deployTiles.Count == 0) return;

            MapEntity nextTile = deployTiles.Cast<DeployTile>().First(tile => tile.DeployTeam == CurrentTurn);
            map.MapCursor.SnapCursorToCoordinates(nextTile.MapCoordinates);
            UpdateHoverView();
            AssetManager.MapUnitCancelSFX.Play();
        }


        private void PassTurn()
        {
            CurrentTurn = OpposingTeam(CurrentTurn);

            List<GameUnit> currentArmy = GetArmy(CurrentTurn);

            if (currentArmy.Count == 0)
            {
                List<GameUnit> opposingArmy = GetArmy(OpposingTeam(CurrentTurn));
                if (opposingArmy.Count == 0)
                {
                    GameContext.CurrentGameState = GameContext.GameState.InGame;
                    GameContext.InitiativeContext.StartFirstTurn();
                    GameMapContext.UpdateWindowsEachTurn();
                }
                else
                {
                    PassTurn();
                }
            }
            else
            {
                currentUnit = currentArmy.First();
            }
        }

        private static Team OpposingTeam(Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    return Team.Red;
                case Team.Red:
                    return Team.Blue;
                default:
                    throw new ArgumentOutOfRangeException("team", team, null);
            }
        }

        private List<GameUnit> GetArmy(Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    return blueArmy;
                case Team.Red:
                    return redArmy;
                default:
                    throw new ArgumentOutOfRangeException("team", team, null);
            }
        }

        private bool TargetTileIsValidDeploymentTile
        {
            get
            {
                MapSlice cursorSlice = map.GetMapSliceAtCursor();
                DeployTile deployTile = cursorSlice.TerrainEntity as DeployTile;

                return deployTile != null && !deployTile.Occupied && deployTile.DeployTeam == currentUnit.Team;
            }
        }
    }
}