using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Uppsalchemy
{
	public class UppsalchemyModSystem : ModSystem
	{
		public override void StartPre(ICoreAPI api)
		{
			base.StartPre(api);

			ConfigServer.Initialize(api);
		}

		public override void Start(ICoreAPI api)
		{
			base.Start(api);

			api.RegisterBlockClass("BlockAlchemyBottle", typeof(BlockAlchemyBottle));
		}

		public override void StartClientSide(ICoreClientAPI api)
		{
			base.StartClientSide(api);

			HudElementAlchemyEffect hud = new HudElementAlchemyEffect(api);
			hud.TryOpen();
		}
	}
}
