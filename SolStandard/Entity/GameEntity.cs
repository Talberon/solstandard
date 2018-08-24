using SolStandard.Map.Elements;

namespace SolStandard.Entity
{
    public abstract class GameEntity
    {
        private readonly string id;
        private readonly MapEntity mapEntity;

        protected GameEntity(string id, ref MapEntity mapEntity)
        {
            this.id = id;
            this.mapEntity = mapEntity;
        }

        public string Id
        {
            get { return id; }
        }

        public MapEntity MapEntity
        {
            get { return mapEntity; }
        }
    }
}