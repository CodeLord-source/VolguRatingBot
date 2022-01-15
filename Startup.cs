using Microsoft.EntityFrameworkCore;
using RatingBot.Bots;
using RatingBot.Models.Db;
using RatingBot.Models.StateMachineActions;
using RatingBot.Services;
using RatingBot.Services.LkVolsuParsing;
using RatingBot.Services.Parser;
using VolguRatingBot.Services.Repository.Interface;

namespace RatingBot
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddSingleton<IRepository, StudentRepository>();
            services.AddSingleton<LkVolsuParser>();
            services.AddSingleton<ParserWorker>();
            services.AddSingleton<IHtmlDocLoader, HtmlDocLoader>();
            services.AddLkVolsuWebRequestSender(configuration);
            services.AddDbContext<StudentContext>(options =>
            options.UseNpgsql("name=ConnectionStrings:DefaultConnection"), ServiceLifetime.Singleton);
            services.AddTelegrammBot(configuration);
            services.AddSingleton<ActionsInitializer>();
            services.AddSingleton<UserDataCheker>();
            services.AddSingleton<DialogStateMachine>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
