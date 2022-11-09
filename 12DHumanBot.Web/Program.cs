using _12DHumanBot.Web.Models;
using AbstractBot;
using Microsoft.Extensions.Options;

namespace _12DHumanBot.Web;

internal static class Program
{
    public static void Main(string[] args)
    {
        Utils.LogManager.DeleteExceptionLog();
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            Config config = Configure(builder);
            Utils.StartLogWith(config.SystemTimeZoneIdLogs);

            IServiceCollection services = builder.Services;
            services.AddControllersWithViews().AddNewtonsoftJson();

            AddBotTo(services);

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors();

            UseUpdateEndpoint(app, config.Token);

            app.Run();
        }
        catch (Exception ex)
        {
            Utils.LogManager.LogException(ex);
        }
    }

    private static Config Configure(WebApplicationBuilder builder)
    {
        ConfigurationManager configuration = builder.Configuration;
        Config config = configuration.Get<Config>();

        builder.Services.AddOptions<Config>().Bind(configuration).ValidateDataAnnotations();
        builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<Config>>().Value);

        return config;
    }

    private static void AddBotTo(IServiceCollection services)
    {
        services.AddSingleton<BotSingleton>();
        services.AddHostedService<BotService>();
    }

    private static void UseUpdateEndpoint(IApplicationBuilder app, string token)
    {
        object defaults = new
        {
            controller = "Update",
            action = "Post"
        };
        app.UseEndpoints(endpoints => endpoints.MapControllerRoute("update", token, defaults));
    }
}