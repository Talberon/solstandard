using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class BuffTile : MapEntity
    {
        public enum Statistic
        {
            Hp,
            Atk,
            Def,
            Sp,
            Mv,
            AtkRange
        }

        private readonly int modifier;
        private readonly Statistic buffStat;
        private readonly bool canMove;

        public BuffTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int modifier, Statistic buffStat, bool canMove) : base(name,
            type, sprite, mapCoordinates, tiledProperties)
        {
            this.modifier = modifier;
            this.buffStat = buffStat;
            this.canMove = canMove;
        }
    }
}