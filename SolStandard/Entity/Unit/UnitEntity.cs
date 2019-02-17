﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Map.Elements;
using SolStandard.Utility;

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
        private static readonly Color InactiveColor = new Color(170, 170, 170);
        private static readonly Color ExhaustedColor = new Color(70, 60, 60);

        private SpriteAtlas commanderCrown;
        private bool isCommander;

        public UnitEntity(string name, string type, UnitSpriteSheet spriteSheet, Vector2 mapCoordinates,
            bool isCommander, Dictionary<string, string> tiledProperties)
            : base(name, type, spriteSheet, mapCoordinates, tiledProperties)
        {
            ElementColor = ActiveColor;

            IsCommander = isCommander;
        }

        public bool IsCommander
        {
            private get { return isCommander; }
            set
            {
                isCommander = value;

                const int crownSize = 8;
                commanderCrown = isCommander ? GameUnit.GetCommanderCrown(new Vector2(crownSize)) : null;
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
            GameUnit thisUnit = UnitSelector.SelectUnit(this);
            return thisUnit.ContextualActions;
        }

        public override void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            if (!Visible) return;

            Sprite.Draw(spriteBatch, EntityRenderPosition, colorOverride);

            if (IsCommander && commanderCrown != null)
            {
                commanderCrown.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize);
            }
        }

        private Vector2 EntityRenderPosition
        {
            get
            {
                return MapCoordinates * GameDriver.CellSize -
                       new Vector2(UnitSpriteSheet.Width, UnitSpriteSheet.Height) / 2 +
                       new Vector2(GameDriver.CellSize) / 2;
            }
        }
    }
}