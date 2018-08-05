using System.Collections.Generic;
using System.Collections.ObjectModel;
using SolStandard.Map.Objects;

namespace SolStandard.Entity
{
    public abstract class GameEntity
    {
        private string id;
        private MapEntity mapEntity;

        protected GameEntity(string id, MapEntity mapEntity)
        {
            this.id = id;
            this.mapEntity = mapEntity;
        }

        public string GetId()
        {
            return id;
        }
    }
}