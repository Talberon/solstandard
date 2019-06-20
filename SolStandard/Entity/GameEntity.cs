using SolStandard.Map.Elements;

namespace SolStandard.Entity
{
    public abstract class GameEntity
    {
        private readonly string id;
        protected MapEntity MapEntity;

        protected GameEntity(string id, MapEntity mapEntity)
        {
            this.id = id;
            MapEntity = mapEntity;
        }

        public string Id
        {
            get { return id; }
        }
    }
}