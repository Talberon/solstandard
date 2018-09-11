using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Chest : MapEntity
    {
        private readonly string contents;
        private bool isLocked;
        private bool isOpen;

        public Chest(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, string contents, bool isLocked, bool isOpen) : base(name, type,
            sprite, mapCoordinates, tiledProperties)
        {
            this.contents = contents;
            this.isLocked = isLocked;
            this.isOpen = isOpen;
        }
    }
}