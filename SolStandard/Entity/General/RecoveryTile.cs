using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class RecoveryTile : TerrainEntity, IEffectTile
    {
        private readonly int amrPerTurn;
        private readonly int hpPerTurn;
        public bool HasTriggered { get; set; }

        public RecoveryTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int amrPerTurn,
            int hpPerTurn) :
            base(name, type, sprite, mapCoordinates)
        {
            this.amrPerTurn = amrPerTurn;
            this.hpPerTurn = hpPerTurn;
            HasTriggered = false;
        }

        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != EffectTriggerTime.StartOfRound || HasTriggered) return false;

            MapSlice recoverySlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
            GameUnit unitOnTile = UnitSelector.SelectUnit(recoverySlice.UnitEntity);

            if (unitOnTile == null) return false;

            GlobalContext.MapCursor.SnapCameraAndCursorToCoordinates(MapCoordinates);
            GlobalContext.MapCamera.SnapCameraCenterToCursor();

            if (hpPerTurn > 0)
            {
                unitOnTile.RecoverHP(hpPerTurn);
                AssetManager.SkillBuffSFX.Play();
            }

            if (amrPerTurn > 0)
            {
                AssetManager.SkillBuffSFX.Play();
                unitOnTile.RecoverArmor(amrPerTurn);
            }

            if (hpPerTurn > 0 || amrPerTurn > 0)
            {
                string toastMessage = "";

                if (hpPerTurn > 0 && amrPerTurn > 0)
                {
                    toastMessage = string.Format("{0} recovers {1} {2}!\n{0} recovers {3} {4}!",
                        unitOnTile.Id,
                        hpPerTurn, UnitStatistics.Abbreviation[Stats.Hp],
                        amrPerTurn, UnitStatistics.Abbreviation[Stats.Armor]
                    );
                }
                else if (hpPerTurn > 0)
                {
                    toastMessage = $"{unitOnTile.Id} recovers {hpPerTurn} {UnitStatistics.Abbreviation[Stats.Hp]}!";
                }
                else if (amrPerTurn > 0)
                {
                    toastMessage = $"{unitOnTile.Id} recovers {amrPerTurn} {UnitStatistics.Abbreviation[Stats.Armor]}!";
                }

                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates(
                    toastMessage,
                    unitOnTile.UnitEntity.MapCoordinates,
                    50
                );
            }

            return true;
        }

        public bool WillTrigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != EffectTriggerTime.StartOfRound || HasTriggered) return false;

            MapSlice recoverySlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
            GameUnit unitOnTile = UnitSelector.SelectUnit(recoverySlice.UnitEntity);
            return unitOnTile != null;
        }

        public bool IsExpired => false;

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Hp, GameDriver.CellSizeVector),
                        new RenderText(AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.Hp] + " Regen: " +
                            ((hpPerTurn > 0) ? "+" : "") + hpPerTurn
                        )
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Armor, GameDriver.CellSizeVector),
                        new RenderText(AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.Armor] + " Regen: " +
                            ((amrPerTurn > 0) ? "+" : "") + amrPerTurn
                        )
                    }
                }
            );
    }
}