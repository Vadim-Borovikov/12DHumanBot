using _12DHumanBot.Web.Models;
using GryphonUtilities;

namespace _12DHumanBot.Web;

internal sealed class Startup
{
    public Startup(IConfiguration config) => _config = config;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<BotSingleton>();
        services.AddHostedService<BotService>();
        services.Configure<ConfigJson>(_config);

        services.AddControllersWithViews().AddNewtonsoftJson();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors();

        ConfigJson botConfig = _config.Get<ConfigJson>();
        string token = botConfig.Token.GetValue(nameof(botConfig.Token));
        object defaults = new
        {
            controller = "Update",
            action = "Post"
        };
        app.UseEndpoints(endpoints => endpoints.MapControllerRoute("update", token, defaults));
    }

    private readonly IConfiguration _config;
}