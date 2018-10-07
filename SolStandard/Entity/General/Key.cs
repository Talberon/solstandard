﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills;
using SolStandard.Entity.Unit.Skills.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Key : TerrainEntity, IItem, IActionTile
    {
        public string UsedWith { get; private set; }
        public int[] Range { get; private set; }

        public Key(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, string usedWith, int[] range) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            UsedWith = usedWith;
            Range = range;
        }

        public IRenderable Icon
        {
            get { return Sprite; }
        }

        public UnitAction TileAction()
        {
            return new PickUpItemAction(this, MapCoordinates);
        }

        public UnitAction UseAction()
        {
            //Find the ILockable map tile that is unlocked with this key
            Vector2 targetEntityCoordinates = Vector2.One;

            foreach (MapEntity element in MapContainer.GameGrid[(int) Layer.Entities])
            {
                if (element is ILockable)
                {
                    if (element.Name == UsedWith)
                    {
                        targetEntityCoordinates = element.MapCoordinates;
                    }
                }
            }

            return new ToggleLockAction(this, targetEntityCoordinates);
        }

        public UnitAction DropAction()
        {
            return new DropItemAction(this);
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
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "Used with: " + UsedWith),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}