using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class BreakableObstacle : TerrainEntity
    {
        private int hp;
        public bool IsBroken { get; private set; }

        public BreakableObstacle(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int hp, bool canMove, bool isBroken) : base(name, type, sprite,
            mapCoordinates, tiledProperties)
        {
            this.hp = hp;
            CanMove = canMove;
            IsBroken = isBroken;
        }

        public void DealDamage(int damage)
        {
            hp -= damage;
            AssetManager.CombatDamageSFX.Play();
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(Name + " takes " + damage + " damage!",
                MapCoordinates, 50);

            if (hp > 0) return;
            
            AssetManager.CombatDeathSFX.Play();
            IsBroken = true;
            CanMove = true;
            Visible = false;

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates("Destroyed!", MapCoordinates,
                50);
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new[,]
                    {
                        {
                            InfoHeader,
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Hp),
                            new RenderText(AssetManager.WindowFont, "HP: " + hp)
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (IsBroken) ? "Broken" : "Not Broken",
                                (IsBroken) ? NegativeColor : PositiveColor),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}