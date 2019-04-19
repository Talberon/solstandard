using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.General
{
    public class RecoveryTile : TerrainEntity, IEffectTile
    {
        private readonly int amrPerturn;
        private readonly int hpPerTurn;

        public RecoveryTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int amrPerturn,
            int hpPerTurn,
            Dictionary<string, string> tiledProperties) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            this.amrPerturn = amrPerturn;
            this.hpPerTurn = hpPerTurn;
        }

        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != EffectTriggerTime.StartOfTurn) return false;

            MapSlice recoverySlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
            GameUnit unitOnTile = UnitSelector.SelectUnit(recoverySlice.UnitEntity);

            if (unitOnTile == null) return false;

            GameContext.MapCursor.SnapCursorToCoordinates(MapCoordinates);
            GameContext.MapCamera.SnapCameraCenterToCursor();

            if (hpPerTurn > 0) GlobalEventQueue.QueueSingleEvent(new RegenerateHealthEvent(unitOnTile, hpPerTurn));
            if (amrPerturn > 0) GlobalEventQueue.QueueSingleEvent(new RegenerateArmorEvent(unitOnTile, hpPerTurn));

            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));

            return true;
        }

        public bool IsExpired
        {
            get { return false; }
        }
    }
}