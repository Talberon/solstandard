using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class BreakableObstacle : TerrainEntity
    {
        private int hp;
        private bool isBroken;

        public BreakableObstacle(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int hp, bool canMove, bool isBroken) : base(name, type, sprite,
            mapCoordinates, tiledProperties)
        {
            this.hp = hp;
            CanMove = canMove;
            this.isBroken = isBroken;
        }

        public void DealDamage(int damage)
        {
            hp -= damage;
            AssetManager.CombatDamageSFX.Play();

            if (hp <= 0)
            {
                AssetManager.CombatDeathSFX.Play();
                isBroken = true;
                CanMove = true;
                Visible = false;
            }
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
                            UnitStatistics.GetSpriteAtlas(StatIcons.Hp),
                            new RenderText(AssetManager.WindowFont, "HP: " + hp)
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (isBroken) ? "Broken" : "Not Broken",
                                (isBroken) ? NegativeColor : PositiveColor),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}