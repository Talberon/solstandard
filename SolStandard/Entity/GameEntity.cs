using SolStandard.Map.Elements;

namespace SolStandard.Entity
{
    public abstract class GameEntity
    {
        protected MapEntity MapEntity;

        protected GameEntity(string id, MapEntity mapEntity)
        {
            Id = id;
            MapEntity = mapEntity;
        }

        public string Id { get; }
    }
}