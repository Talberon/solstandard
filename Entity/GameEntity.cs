using System.Collections.Generic;
using System.Collections.ObjectModel;
using SolStandard.Map.Objects;
using SolStandard.Map.Objects.EntityProps;

namespace SolStandard.Entity
{
    public abstract class GameEntity
    {
        private string id;
        private MapEntity mapEntity;
        private readonly List<EntityProp> properties;

        protected GameEntity(string id, MapEntity mapEntity, List<EntityProp> properties)
        {
            this.id = id;
            this.mapEntity = mapEntity;
            this.properties = properties;
        }

        public string GetId()
        {
            return id;
        }

        public ReadOnlyCollection<EntityProp> GetProperties()
        {
            return properties.AsReadOnly();
        }
    }
}