using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Door : MapEntity
    {
        private bool isLocked;
        private bool isOpen;

        public Door(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool isLocked, bool isOpen) : base(name, type, sprite,
            mapCoordinates, tiledProperties)
        {
            this.isLocked = isLocked;
            this.isOpen = isOpen;
        }
    }
}