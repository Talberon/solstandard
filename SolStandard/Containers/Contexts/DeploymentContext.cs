using System;
using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.View;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;

namespace SolStandard.Containers.Contexts
{
    public class DeploymentContext
    {
        public DeploymentView DeploymentView { get; private set; }
        private readonly List<GameUnit> blueArmy;
        private readonly List<GameUnit> redArmy;
        private readonly MapContainer map;
        private GameUnit currentUnit;
        private Team currentTurn;

        public DeploymentContext(List<GameUnit> blueArmy, List<GameUnit> redArmy, MapContainer map, Team firstTurn)
        {
            this.blueArmy = blueArmy;
            this.redArmy = redArmy;
            this.map = map;
            currentTurn = firstTurn;
            currentUnit = GetArmy(currentTurn).First();
            DeploymentView = new DeploymentView();
            //TODO Load the map,
            //TODO select a team to deploy a unit on a deployment tile
            //TODO alternate until all units are deployed
            //TODO start the GameMapContext
        }

        public void SelectNextUnit()
        {
            List<GameUnit> activeArmy = GetArmy(currentTurn);
            int currentUnitIndex = activeArmy.IndexOf(currentUnit);

            int nextIndex = (currentUnitIndex + 1 > activeArmy.Count) ? 0 : currentUnitIndex + 1;

            currentUnit = activeArmy[nextIndex];
        }

        public void SelectPreviousUnit()
        {
            List<GameUnit> activeArmy = GetArmy(currentTurn);
            int currentUnitIndex = activeArmy.IndexOf(currentUnit);

            int nextIndex = (currentUnitIndex - 1 < 0) ? activeArmy.Count - 1 : currentUnitIndex - 1;

            currentUnit = activeArmy[nextIndex];
        }

        public void TryDeployUnit()
        {
            if (TargetTileIsValidDeploymentTile)
            {
                PlaceUnitInTile();
                currentTurn = OpposingTeam(currentTurn);
            }
            else
            {
                map.AddNewToastAtMapCursor("Place a unit at an unoccupied deployment tile for your team!", 50);
            }
        }

        private void PlaceUnitInTile()
        {
            currentUnit.UnitEntity.MapCoordinates = map.MapCursor.MapCoordinates;
            GameContext.Units.Add(currentUnit);
            GetArmy(currentUnit.Team).Remove(currentUnit);
        }

        public void MoveCursorOnMap(Direction direction)
        {
            map.MapCursor.MoveCursorInDirection(direction);
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