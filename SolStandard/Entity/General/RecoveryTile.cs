using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class RecoveryTile : TerrainEntity, IEffectTile
    {
        private readonly int amrPerTurn;
        private readonly int hpPerTurn;

        public RecoveryTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int amrPerTurn,
            int hpPerTurn) :
            base(name, type, sprite, mapCoordinates)
        {
            this.amrPerTurn = amrPerTurn;
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

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                    toastMessage,
                    unitOnTile.UnitEntity.MapCoordinates,
                    50
                );
            }

            return true;
        }

        public bool WillTrigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != EffectTriggerTime.StartOfTurn) return false;

            MapSlice recoverySlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
            GameUnit unitOnTile = UnitSelector.SelectUnit(recoverySlice.UnitEntity);
            return unitOnTile != null;
        }

        public bool IsExpired => false;

        public override IRenderable TerrainInfo
        {
            get
            {
                IRenderable[,] statContent =
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Hp, new Vector2(GameDriver.CellSize)),
                        new RenderText(AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.Hp] + " Regen: " +
                            ((hpPerTurn > 0) ? "+" : "") + hpPerTurn
                        )
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Armor, new Vector2(GameDriver.CellSize)),
                        new RenderText(AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.Armor] + " Regen: " +
                            ((amrPerTurn > 0) ? "+" : "") + amrPerTurn
                        )
                    }
                };

                Window statContentWindow = new Window(statContent, InnerWindowColor);

                return new WindowContentGrid(
                    new[,]
                    {
                        {
                            InfoHeader,
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            statContentWindow,
                            new RenderBlank()
                        }
                    },
                    3,
                    HorizontalAlignment.Centered
                );
            }
        }
    }
}