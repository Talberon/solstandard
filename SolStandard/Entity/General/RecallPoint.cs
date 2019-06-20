using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class RecallPoint : TerrainEntity
    {
        private readonly string recallId;

        public RecallPoint(string recallId, IRenderable sprite, Vector2 mapCoordinates) :
            base(recallId + " Point", "RecallPoint", sprite, mapCoordinates)
        {
            this.recallId = recallId;
        }

        // ReSharper disable once ParameterHidesMember
        public bool BelongsToSource(string recallId)
        {
            return recallId.Equals(this.recallId);
        }
    }
}