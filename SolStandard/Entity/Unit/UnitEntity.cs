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

        private static readonly Color ActiveColor = Color.White;
        private static readonly Color InactiveColor = new Color(190, 190, 190);
        private static readonly Color ExhaustedColor = new Color(160,160,160);

        private SpriteAtlas commanderCrown;
        private bool isCommander;
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
        }


        public bool IsCommander
        {
            get => isCommander;
            set
            {
                isCommander = value;

                const int crownSize = 8;
                commanderCrown =
                    isCommander ? MiscIconProvider.GetMiscIcon(MiscIcon.Crown, new Vector2(crownSize)) : null;
            }
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
                case UnitEntityState.Exhausted:
                    ElementColor = ExhaustedColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
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
        }

        private Vector2 EntityRenderPosition =>
            CurrentDrawCoordinates -
            new Vector2(UnitSpriteSheet.Width, UnitSpriteSheet.Height) / 2 +
            GameDriver.CellSizeVector / 2;
    }
}