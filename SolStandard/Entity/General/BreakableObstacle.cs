using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class BreakableObstacle : TerrainEntity
    {
        protected int HP { get; private set; }
        public bool IsBroken { get; private set; }
        private int gold;
        private readonly List<IItem> items;

        public BreakableObstacle(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int hp,
            bool canMove, bool isBroken, int gold, IItem item = null) :
            base(name, type, sprite, mapCoordinates)
        {
            HP = hp;
            CanMove = canMove;
            IsBroken = isBroken;
            this.gold = gold;

            items = new List<IItem>();
            if (item != null) items.Add(item);
        }

        public void DealDamage(int damage)
        {
            HP -= damage;
            AssetManager.CombatDamageSFX.Play();
            GlobalContext.WorldContext.PlayAnimationAtCoordinates(
                AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.Damage, GameDriver.CellSizeVector),
                MapCoordinates
            );
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates(
                Name + " takes " + damage + " damage!",
                MapCoordinates, 50);

            if (HP > 0) return;

            DropSpoils();
            AssetManager.CombatDeathSFX.Play();
            IsBroken = true;
            CanMove = true;
            Visible = false;

            //Remove self from the map
            MapContainer.GameGrid[(int) Layer.Entities][(int) MapCoordinates.X, (int) MapCoordinates.Y] = null;

            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates("Destroyed!", MapCoordinates, 50);
        }

        private void DropSpoils()
        {
            //Don't drop spoils if inventory is empty
            if (gold == 0 && items.Count == 0) return;

            //If on top of other Spoils, pick those up before dropping on top of them

            if (MapContainer.GameGrid[(int) Layer.Items][(int) MapCoordinates.X,
                (int) MapCoordinates.Y] is Spoils spoilsAtUnitPosition)
            {
                gold += spoilsAtUnitPosition.Gold;
                items.AddRange(spoilsAtUnitPosition.Items);
            }


            //Check if an item already exists here and add it to the spoils so that they aren't lost 
            if (MapContainer.GameGrid[(int) Layer.Items][(int) MapCoordinates.X,
                (int) MapCoordinates.Y] is TerrainEntity itemAtUnitPosition)
            {
                switch (itemAtUnitPosition)
                {
                    case IItem item:
                        items.Add(item);
                        break;
                    case Currency groundGold:
                    {
                        gold += groundGold.Value;
                        break;
                    }
                }
            }

            MapContainer.GameGrid[(int) Layer.Items][(int) MapCoordinates.X, (int) MapCoordinates.Y]
                = new Spoils(
                    Name + " Spoils",
                    "Spoils",
                    MiscIconProvider.GetMiscIcon(MiscIcon.Spoils, GameDriver.CellSizeVector),
                    MapCoordinates,
                    gold,
                    new List<IItem>(items)
                );

            gold = 0;
            items.Clear();
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(
                new[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Hp),
                        new RenderText(AssetManager.WindowFont, "HP: " + HP)
                    },
                    {
                        new RenderText(AssetManager.WindowFont, (IsBroken) ? "Broken" : "Not Broken",
                            (IsBroken) ? NegativeColor : PositiveColor),
                        RenderBlank.Blank
                    }
                }
            );
    }
}