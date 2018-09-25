﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Map.Elements;

namespace SolStandard.Entity.Unit
{
    public class UnitEntity : MapEntity
    {
        public enum UnitEntityState
        {
            Active,
            Inactive
        }

        private static readonly Color ActiveColor = Color.White;
        private static readonly Color InactiveColor = new Color(200, 200, 200);

        public UnitEntity(string name, string type, UnitSprite sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties) : base(name, type, sprite, mapCoordinates, tiledProperties)
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

        public UnitSprite UnitSprite
        {
            get { return (UnitSprite) Sprite; }
        }

        public override void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            Sprite.Draw(spriteBatch,
                MapCoordinates * GameDriver.CellSize - new Vector2(UnitSprite.Width, UnitSprite.Height) / 2 + new Vector2(GameDriver.CellSize) / 2,
                
                colorOverride);
        }
    }
}