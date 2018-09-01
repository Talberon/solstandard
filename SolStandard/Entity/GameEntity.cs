using SolStandard.Map.Elements;

namespace SolStandard.Entity
{
    public abstract class GameEntity
    {
        private readonly string id;
        public MapEntity MapEntity { get; protected set; }

        protected GameEntity(string id, ref MapEntity mapEntity)
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