using SolStandard.Map.Elements;

namespace SolStandard.Entity
{
    public abstract class GameEntity
    {
        private readonly string id;
        private readonly MapEntity mapInfo;

        protected GameEntity(string id, MapEntity mapInfo)
        {
            this.id = id;
            this.mapInfo = mapInfo;
        }

        public string Id
        {
            get { return id; }
        }

        public MapEntity MapInfo
        {
            get { return mapInfo; }
        }
    }
}