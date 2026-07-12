using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Uppsalchemy
{
    public interface IAlchemyEffectHandler
    {
        void Apply(JsonObject effect, EntityAgent byEntity);
        void Clear(EntityAgent byEntity);

        void Tick(EntityAgent byEntity);
    }
}