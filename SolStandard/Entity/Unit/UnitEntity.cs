using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Map.Elements;

namespace SolStandard.Entity.Unit
{
    public class UnitEntity : MapEntity, IActionTile
    {
        public enum UnitEntityState
        {
            Active,
            Inactive
        }

        private static readonly Color ActiveColor = Color.White;
        private static readonly Color InactiveColor = new Color(180, 180, 180);

        public UnitEntity(string name, string type, UnitSpriteSheet spriteSheet, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties) : base(name, type, spriteSheet, mapCoordinates, tiledProperties)
        {
            ElementColor = ActiveColor;
        }

        public void SetState(UnitEntityState state)
        {
            switch (state)
            {
                case UnitEntityState.Active:
                    ElementColor = ActiveColor;
                    break;
                case UnitEntityState.Inactive:
                    ElementColor = InactiveColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("state", state, null);
            }
        }

        public UnitSpriteSheet UnitSpriteSheet
        {
            get { return (UnitSpriteSheet) Sprite; }
        }

        public int[] InteractRange
        {
            get { return new[] {1}; }
        }

        public List<UnitAction> TileActions()
        {
            //TODO Introduce contextual actions for proximity to certain units
            return new List<UnitAction>();
        }

        public override void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            if (Visible)
            {
                Sprite.Draw(
                    spriteBatch,
                    MapCoordinates * GameDriver.CellSize -
                    new Vector2(UnitSpriteSheet.Width, UnitSpriteSheet.Height) / 2 +
                    new Vector2(GameDriver.CellSize) / 2,
                    colorOverride
                );
            }
        }
    }
}