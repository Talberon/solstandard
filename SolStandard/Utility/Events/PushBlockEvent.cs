﻿using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Map;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class PushBlockEvent : IEvent
    {
        private readonly PushBlock target;

        public PushBlockEvent(PushBlock target)
        {
            this.target = target;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            Vector2 actorCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = target.MapCoordinates;
            Vector2 oppositeCoordinates = UnitAction.DetermineOppositeTileOfUnit(actorCoordinates, targetCoordinates);
            MoveTerrainEntityToPosition(Layer.Entities, targetCoordinates, oppositeCoordinates);
            AssetManager.CombatBlockSFX.Play();
            Complete = true;
        }

        private static void MoveTerrainEntityToPosition(Layer mapLayer, Vector2 startPosition, Vector2 destination)
        {
            TerrainEntity entityToMove =
                MapContainer.GameGrid[(int) mapLayer][(int) startPosition.X, (int) startPosition.Y] as TerrainEntity;

            if (entityToMove != null) entityToMove.SlideToCoordinates(destination);

            MapContainer.GameGrid[(int) mapLayer][(int) destination.X, (int) destination.Y] = entityToMove;
            MapContainer.GameGrid[(int) mapLayer][(int) startPosition.X, (int) startPosition.Y] = null;
        }
    }
}