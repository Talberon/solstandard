using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public abstract class UnitSkill
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        protected readonly SpriteAtlas TileSprite;

        protected UnitSkill(string name, string description, SpriteAtlas tileSprite)
        {
            Name = name;
            Description = description;
            TileSprite = tileSprite;
        }

        public abstract void GenerateActionGrid(Vector2 origin);
        public abstract void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext);

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void CancelAction(MapContext mapContext)
        {
            MapContainer.ClearDynamicGrid();
            mapContext.RevertToPreviousState();
            AssetManager.MapUnitCancelSFX.Play();
        }
        
        protected static void SkipCombatPhase(MapContext mapContext)
        {
            EnterCombatPhase(mapContext);
            mapContext.ProceedToNextState();
        }

        protected static void EnterCombatPhase(MapContext mapContext)
        {
            mapContext.ProceedToNextState();
            mapContext.SetPromptWindowText("Confirm End Turn");
        }
    }
}