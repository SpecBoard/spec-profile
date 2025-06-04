using STrain.CQS.NetCore;
using STrain.CQS.NetCore.Builders;
using STrain.Eventing.KurrentDB.NetCore.Extensions;

namespace SpecProfile.Web.Wireups
{
	public static class Wireups
	{
		public static void UseCQS(this WebApplicationBuilder builder)
		{
			builder.AddCQS(builder =>
			{
				builder.AddGenericRequestHandler()
				.AddMvcRequestReceiver();

				builder.AddRequestValidator()
					.UseFluentRequestValidator(builder => { });
			});
		}

		public static void UseEventing(this WebApplicationBuilder builder)
		{
			builder.AddEventing(builder => builder.AddKurrentDB((settings, configuration) => configuration.Bind("KurrentDB", settings)));
		}
	}
}
