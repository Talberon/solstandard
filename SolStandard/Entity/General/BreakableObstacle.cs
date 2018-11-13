using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class BreakableObstacle : TerrainEntity
    {
        private int hp;
        public bool IsBroken { get; private set; }
        private int gold;
        private readonly List<IItem> items;

        public BreakableObstacle(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int hp, bool canMove, bool isBroken, int gold,
            IItem item = null) : base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            this.hp = hp;
            CanMove = canMove;
            IsBroken = isBroken;
            this.gold = gold;

            items = new List<IItem>();
            if (item != null) items.Add(item);
        }

        public void DealDamage(int damage)
        {
            hp -= damage;
            AssetManager.CombatDamageSFX.Play();
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                Name + " takes " + damage + " damage!",
                MapCoordinates, 50);

            if (hp > 0) return;

            DropSpoils();
            AssetManager.CombatDeathSFX.Play();
            IsBroken = true;
            CanMove = true;
            Visible = false;

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates("Destroyed!", MapCoordinates, 50);
        }

        private void DropSpoils()
        {
            //Don't drop spoils if inventory is empty
            if (gold == 0 && items.Count == 0) return;

            //If on top of other Spoils, pick those up before dropping on top of them
            Spoils spoilsAtUnitPosition =
                MapContainer.GameGrid[(int) Layer.Items][(int) MapCoordinates.X,
                    (int) MapCoordinates.Y] as Spoils;

            if (spoilsAtUnitPosition != null)
            {
                gold += spoilsAtUnitPosition.Gold;
                items.AddRange(spoilsAtUnitPosition.Items);
            }


            TerrainEntity itemAtUnitPosition =
                MapContainer.GameGrid[(int) Layer.Items][(int) MapCoordinates.X,
                    (int) MapCoordinates.Y] as TerrainEntity;

            //Check if an item already exists here and add it to the spoils so that they aren't lost 
            if (itemAtUnitPosition != null)
            {
                IItem item = itemAtUnitPosition as IItem;
                if (item != null)
                {
                    items.Add(item);
                }
                else if (itemAtUnitPosition is Currency)
                {
                    Currency groundGold = itemAtUnitPosition as Currency;
                    gold += groundGold.Value;
                }
            }

            MapContainer.GameGrid[(int) Layer.Items][(int) MapCoordinates.X, (int) MapCoordinates.Y]
                = new Spoils(
                    Name + " Spoils",
                    "Spoils",
                    new SpriteAtlas(AssetManager.SpoilsIcon, new Vector2(GameDriver.CellSize)),
                    MapCoordinates,
                    new Dictionary<string, string>(),
                    gold,
                    new List<IItem>(items)
                );

            gold = 0;
            items.Clear();
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