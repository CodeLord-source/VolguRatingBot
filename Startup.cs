using Microsoft.EntityFrameworkCore;
using RatingBot.Bots;
using RatingBot.Bots.Telegram;
using RatingBot.Models;
using RatingBot.Services;
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
            services.AddSingleton<DialogStateMachine>();
            services.AddSingleton<IRepository, StudentRepository>(); 
            services.AddDbContext<StudentContext>(options =>
            options.UseNpgsql("name=ConnectionStrings:DefaultConnection"),ServiceLifetime.Singleton);
            services.AddTelegrammBot(configuration); 
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
