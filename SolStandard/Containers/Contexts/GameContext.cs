using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts
{
    public class GameContext
    {
        private readonly MapContext mapContext;
        private readonly BattleContext battleContext;
        private static InitiativeContext _initiativeContext;

        public GameContext(MapContext mapContext, BattleContext battleContext, InitiativeContext initiativeContext)
        {
            this.mapContext = mapContext;
            this.battleContext = battleContext;
            _initiativeContext = initiativeContext;
        }

        public static List<GameUnit> Units
        {
            get { return _initiativeContext.InitiativeList; }
        }

        public static GameUnit ActiveUnit
        {
            get { return _initiativeContext.CurrentActiveUnit; }
        }

        public MapContext MapContext
        {
            get { return mapContext; }
        }

        public BattleContext BattleContext
        {
            get { return battleContext; }
        }

        public InitiativeContext InitiativeContext
        {
            get { return _initiativeContext; }
        }

        public void SelectUnitAndStartMoving()
        {
            if (!TrySelectUnit()) return;
            StartMoving();
        }

        public void FinishMoving()
        {
            if (MapContext.OtherUnitExistsAtCursor()) return;
            MapContext.ProceedToNextState();

            MapContext.MapContainer.ClearDynamicGrid();
            MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
            //TODO Open the action menu
        }

        public void StartAction()
        {
            //TODO Select option in the action menu
            MapContext.ProceedToNextState();
            MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
            //If the selection is Basic Attack
            //Open the targeting grid
            MapContext.SelectedUnit =
                UnitSelector.SelectUnit(
                    MapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);
            MapContext.GenerateTargetingGridAtUnit(new SpriteAtlas(
                new Texture2DWrapper(GameDriver.TerrainTextures.MonoGameTexture),
                new Vector2(GameDriver.CellSize), 68));
        }

        public void StartCombat()
        {
            GameUnit attackingUnit = MapContext.SelectedUnit;
            GameUnit defendingUnit = UnitSelector.SelectUnit(MapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);

            if (MapContext.TargetUnitIsLegal(defendingUnit))
            {
                MapContext.MapContainer.ClearDynamicGrid();
                BattleContext.StartNewCombat(attackingUnit,
                    MapContext.MapContainer.GetMapSliceAtCoordinates(attackingUnit.UnitEntity.MapCoordinates),
                    defendingUnit,
                    MapContext.MapContainer.GetMapSliceAtCoordinates(defendingUnit.UnitEntity.MapCoordinates));

                MapContext.SetPromptWindowText("Confirm End Turn");
                MapContext.ProceedToNextState();
            }
            //Skip the combat state if player selects the same unit
            else if (attackingUnit == defendingUnit)
            {
                MapContext.MapContainer.ClearDynamicGrid();
                MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                MapContext.ProceedToNextState();
                MapContext.SetPromptWindowText("Confirm End Turn");
                MapContext.ProceedToNextState();
            }
        }

        public void ContinueCombat()
        {
            switch (BattleContext.CurrentState)
            {
                case BattleContext.BattleState.Start:
                    if (BattleContext.TryProceedToNextState())
                        BattleContext.StartRollingDice();
                    break;
                case BattleContext.BattleState.RollDice:
                    if (BattleContext.TryProceedToNextState())
                        BattleContext.StartResolvingBlocks();
                    break;
                case BattleContext.BattleState.CountDice:
                    if (BattleContext.TryProceedToNextState())
                        BattleContext.StartResolvingDamage();
                    break;
                case BattleContext.BattleState.ResolveCombat:
                    if (BattleContext.TryProceedToNextState())
                    {
                        MapContext.ProceedToNextState();
                    }

                    break;
                default:
                    MapContext.ProceedToNextState();
                    return;
            }
        }

        public void ResolveTurn()
        {
            //TODO Do various turn check resolution (win state, etc.)
            MapContext.ConfirmPromptWindow();
            ActiveUnit.DisableExhaustedUnit();
            InitiativeContext.PassTurnToNextUnit();
            ActiveUnit.ActivateUnit();
            ActiveUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
            MapContext.ResetCursorToActiveUnit();
            MapContext.EndTurn();
        }


        public void StartGame()
        {
            foreach (GameUnit unit in Units)
            {
                unit.DisableExhaustedUnit();
            }
            
            ActiveUnit.ActivateUnit();
            ActiveUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
            MapContext.ResetCursorToActiveUnit();
            MapContext.EndTurn();
        }

        public void CancelMove()
        {
            MapContext.CancelMovement();
        }

        public void EndTurnIfUnitIsDead()
        {
            if (MapContext.CurrentTurnState == MapContext.TurnState.SelectUnit && ActiveUnit.UnitEntity == null)
            {
                ResolveTurn();
            }
        }

        private void StartMoving()
        {
            if (MapContext.SelectedUnit != null)
            {
                Trace.WriteLine("Selecting unit: " + MapContext.SelectedUnit.UnitTeam + " " +
                                MapContext.SelectedUnit.UnitJobClass);
                MapContext.ProceedToNextState();
                MapContext.GenerateMoveGrid(
                    MapContext.MapContainer.MapCursor.MapCoordinates,
                    MapContext.SelectedUnit.Stats.MaxMv,
                    new SpriteAtlas(new Texture2DWrapper(GameDriver.TerrainTextures.MonoGameTexture),
                        new Vector2(GameDriver.CellSize), 69));
            }
            else
            {
                Trace.WriteLine("No unit to select.");
            }
        }

        private bool TrySelectUnit()
        {
            //Select the unit. Store it somewhere.
            MapContext.SelectedUnit =
                UnitSelector.SelectUnit(
                    MapContext.MapContainer.GetMapSliceAtCursor().UnitEntity);

            //If the entity selected isn't the active unit, don't select it.
            if (MapContext.SelectedUnit != ActiveUnit)
            {
                MapContext.SelectedUnit = null;
                //TODO Notify the player the selected unit is not legal
                return false;
            }

            return true;
        }
    }
}