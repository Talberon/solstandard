using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit
{
    public class UnitEntity : MapEntity, IActionTile
    {
        public enum UnitEntityState
        {
            Active,
            Inactive,
            Exhausted
        }

        private const int OverlayIconSize = 8;
        private static readonly Color ActiveColor = Color.White;
        private static readonly Color InactiveColor = new Color(190, 190, 190);
        private static readonly Color ExhaustedColor = new Color(160, 160, 160);

        private SpriteAtlas commanderCrown;

        private static readonly SpriteAtlas SpoilsIcon =
            MiscIconProvider.GetMiscIcon(MiscIcon.Spoils, new Vector2(OverlayIconSize));

        private bool isCommander;
        public bool HasItemsInInventory { get; set; }
        public Team Team { get; }
        public Role Role { get; }
        public string[] InitialInventory { get; }

        public UnitEntity(string name, string type, UnitSpriteSheet spriteSheet, Vector2 mapCoordinates, Team team,
            Role role, bool isCommander, string[] initialInventory)
            : base(name, type, spriteSheet, mapCoordinates)
        {
            ElementColor = ActiveColor;
            Team = team;
            Role = role;
            IsCommander = isCommander;
            InitialInventory = initialInventory;
            HasItemsInInventory = initialInventory.Length > 0;
        }


        public bool IsCommander
        {
            get => isCommander;
            set
            {
                isCommander = value;

                commanderCrown =
                    isCommander ? MiscIconProvider.GetMiscIcon(MiscIcon.Crown, new Vector2(OverlayIconSize)) : null;
            }
        }

        public void SetState(UnitEntityState state)
        {
            ElementColor = state switch
            {
                UnitEntityState.Active => ActiveColor,
                UnitEntityState.Inactive => InactiveColor,
                UnitEntityState.Exhausted => ExhaustedColor,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }

        public UnitSpriteSheet UnitSpriteSheet => (UnitSpriteSheet) Sprite;

        public int[] InteractRange => new[] {1};

        public List<UnitAction> TileActions()
        {
            GameUnit thisUnit = UnitSelector.SelectUnit(this);
            return thisUnit.ContextualActions;
        }

        protected override void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            UpdateRenderCoordinates();

            if (!Visible) return;

            Sprite.Draw(spriteBatch, EntityRenderPosition, colorOverride);

            if (IsCommander)
            {
                commanderCrown?.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize);
            }

            if (HasItemsInInventory)
            {
                var topRightOfTile = new Vector2(
                    MapCoordinates.X * GameDriver.CellSize + GameDriver.CellSize - SpoilsIcon.Width,
                    MapCoordinates.Y * GameDriver.CellSize
                );

                SpoilsIcon.Draw(spriteBatch, topRightOfTile);
            }
        }

        private Vector2 EntityRenderPosition =>
            CurrentDrawCoordinates -
            new Vector2(UnitSpriteSheet.Width, UnitSpriteSheet.Height) / 2 +
            GameDriver.CellSizeVector / 2;
    }
}