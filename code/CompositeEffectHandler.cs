using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Uppsalchemy
{
	public class CompositeEffectHandler : IAlchemyEffectHandler
	{
		readonly IAlchemyEffectHandler[] children;

		public CompositeEffectHandler(params IAlchemyEffectHandler[] children)
		{
			this.children = children;
		}

		public void Apply(JsonObject effect, EntityAgent byEntity)
		{
			foreach (var child in children)
			{
				child.Apply(effect, byEntity);
			}
		}

		public void Tick(EntityAgent byEntity)
		{
			foreach (var child in children)
			{
				child.Tick(byEntity);
			}
		}

		public void Clear(EntityAgent byEntity)
		{
			foreach (var child in children)
			{
				child.Clear(byEntity);
			}
		}
	}
}
