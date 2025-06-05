using SpecProfile.Application;
using SpecProfile.Application.Repositories;
using SpecProfile.Commands;
using STrain;
using STrain.CQS.NetCore;
using STrain.CQS.NetCore.Builders;
using STrain.Eventing.KurrentDB.NetCore.Extensions;

namespace SpecProfile.Web.Wireups
{
    public static class Wireups
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IUserRepository, UserRepository>();
        }

        public static void AddCQS(this WebApplicationBuilder builder)
        {
            builder.AddCQS(builder =>
            {
                builder.AddGenericRequestHandler()
                .AddMvcRequestReceiver();

                builder.AddRequestValidator()
                    .UseFluentRequestValidator(builder => { });

                builder.AddPerformer<ICommandPerformer<LogInCommand>, LogInCommandPerformer>();
            });
        }

        public static void AddEventing(this WebApplicationBuilder builder)
        {
            builder.AddEventing(builder => builder.AddKurrentDB((settings, configuration) => configuration.Bind("KurrentDB", settings)));
        }
    }
}
