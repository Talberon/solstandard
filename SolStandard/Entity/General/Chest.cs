﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    //TODO implement IActionTile interface
    public class Chest : TerrainEntity
    {
        private readonly string contents;
        private bool isLocked;
        private bool isOpen;
        private bool canMove;
        public int[] Range { get; private set; }

        public Chest(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, string contents, bool isLocked, bool isOpen,
            bool canMove, int[] range) : base(name, type,
            sprite, mapCoordinates, tiledProperties)
        {
            this.contents = contents;
            this.isLocked = isLocked;
            this.isOpen = isOpen;
            this.canMove = canMove;
            Range = range;
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new[,]
                    {
                        {
                            Sprite,
                            new RenderText(AssetManager.HeaderFont, Name)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "~~~~~~~~~~~"),
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, (canMove) ? "Can Move" : "No Move",
                                (canMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (isLocked) ? "Locked" : "Unlocked",
                                (isLocked) ? NegativeColor : PositiveColor),
                            new RenderBlank()
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (isOpen) ? "Open" : "Closed",
                                (isOpen) ? PositiveColor : NegativeColor),
                            new RenderBlank()
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "Contents: "),
                            new RenderText(AssetManager.WindowFont, (isOpen) ? contents : "????")
                        }
                    },
                    3
                );
            }
        }

        public UnitSkill TileAction()
        {
            throw new NotImplementedException();
        }
    }
}