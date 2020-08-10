using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Containers.Components.World.SubContext.Targeting;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Components.Deployment
{
    public class DeploymentContext 
    {
        public DeploymentHUD DeploymentHUD { get; }
        private readonly List<GameUnit> blueArmy;
        private readonly List<GameUnit> redArmy;
        private readonly MapContainer map;
        private GameUnit currentUnit;
        public Team CurrentTurn { get; private set; }
        public bool CanPressConfirm => HoveringOverDeployTile;

        private bool HoveringOverDeployTile =>
            GlobalContext.WorldContext.MapContainer.GetMapSliceAtCursor().TerrainEntity is DeployTile;

        public DeploymentContext(List<GameUnit> blueArmy, List<GameUnit> redArmy, MapContainer map, Team firstTurn)
        {
            this.blueArmy = blueArmy;
            this.redArmy = redArmy;
            this.map = map;
            CurrentTurn = firstTurn;
            currentUnit = GetArmy(CurrentTurn).First();
            DeploymentHUD = new DeploymentHUD(blueArmy, redArmy, currentUnit, GlobalContext.Scenario);
            MoveToNextDeploymentTile();
        }

        public void SelectNextUnit()
        {
            AssetManager.MapUnitCancelSFX.Play();

            List<GameUnit> activeArmy = GetArmy(CurrentTurn);
            int currentUnitIndex = activeArmy.IndexOf(currentUnit);

            int nextIndex = (currentUnitIndex + 1 > activeArmy.Count - 1) ? 0 : currentUnitIndex + 1;

            currentUnit = activeArmy[nextIndex];
            DeploymentHUD.UpdateRosterLists(blueArmy, redArmy, currentUnit);
        }

        public void SelectPreviousUnit()
        {
            AssetManager.MapUnitCancelSFX.Play();

            List<GameUnit> activeArmy = GetArmy(CurrentTurn);
            int currentUnitIndex = activeArmy.IndexOf(currentUnit);

            int nextIndex = (currentUnitIndex - 1 < 0) ? activeArmy.Count - 1 : currentUnitIndex - 1;

            currentUnit = activeArmy[nextIndex];
            DeploymentHUD.UpdateRosterLists(blueArmy, redArmy, currentUnit);
        }

        public void TryDeployUnit()
        {
            if (TargetTileIsValidDeploymentTile)
            {
                AssetManager.MapUnitCancelSFX.Play();
                PlaceUnitInTile();
                PassTurn();
                DeploymentHUD.UpdateRosterLists(blueArmy, redArmy, currentUnit);
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

                currentUnit.UnitEntity.SnapToCoordinates(map.MapCursor.MapCoordinates);
            }

            GlobalContext.Units.Add(currentUnit);
            GetArmy(currentUnit.Team).Remove(currentUnit);
            DeploymentHUD.UpdateRosterLists(blueArmy, redArmy, currentUnit);
        }

        public void MoveCursorOnMap(Direction direction)
        {
            map.MapCursor.MoveCursorInDirection(direction);
            UpdateHoverView();
        }

        private void UpdateHoverView()
        {
            MapSlice hoverSlice = GlobalContext.WorldContext.MapContainer.GetMapSliceAtCursor();
            GameUnit hoverUnit = UnitSelector.SelectUnit(hoverSlice.UnitEntity);

            MapContainer.ClearDynamicAndPreviewGrids();

            if (hoverUnit != null)
            {
                new UnitTargetingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                    .GenerateThreatGrid(hoverSlice.MapCoordinates, hoverUnit, hoverUnit.Team);
            }

            DeploymentHUD.UpdateHoverUnitWindows(hoverUnit);
            UpdateItemPreview(hoverSlice);

            DeploymentHUD.SetEntityWindow(hoverSlice);
        }

        private void UpdateItemPreview(MapSlice hoverSlice)
        {
            List<IItem> items = WorldContext.CollectItemsFromSlice(hoverSlice);

            Color windowColor = WorldHUD.ItemTerrainWindowColor;

            GameUnit hoverUnit = UnitSelector.SelectUnit(hoverSlice.UnitEntity);
            if (hoverUnit != null)
            {
                windowColor = TeamUtility.DetermineTeamWindowColor(hoverUnit.Team);
            }

            if (items.Count > 0)
            {
                DeploymentHUD.GenerateItemDetailWindow(items, windowColor);
            }
            else
            {
                DeploymentHUD.CloseItemDetailWindow();
            }
        }

        public void MoveToNextDeploymentTile()
        {
            IEnumerable<MapEntity> mapEntities = MapContainer.GetMapEntities();
            List<MapEntity> deployTiles = mapEntities.Where(tile => tile is DeployTile).ToList();
            if (deployTiles.Count == 0) return;

            MapEntity nextTile = deployTiles.Cast<DeployTile>().First(tile => tile.DeployTeam == CurrentTurn);
            map.MapCursor.SnapCameraAndCursorToCoordinates(nextTile.MapCoordinates);
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
                    GlobalContext.CurrentGameState = GlobalContext.GameState.InGame;
                    GlobalContext.InitiativePhase.StartFirstTurn();
                    WorldContext.UpdateWindowsEachTurn();
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
            return team switch
            {
                Team.Blue => Team.Red,
                Team.Red => Team.Blue,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };
        }

        private List<GameUnit> GetArmy(Team team)
        {
            return team switch
            {
                Team.Blue => blueArmy,
                Team.Red => redArmy,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };
        }

        private bool TargetTileIsValidDeploymentTile
        {
            get
            {
                MapSlice cursorSlice = map.GetMapSliceAtCursor();

                return cursorSlice.TerrainEntity is DeployTile deployTile && !deployTile.Occupied &&
                       deployTile.DeployTeam == currentUnit.Team;
            }
        }public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}