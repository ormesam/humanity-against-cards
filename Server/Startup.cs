using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Game;
using Server.Hubs;

namespace Server {
    public class Startup {
        private readonly string AllowSpecificOrigins = "allowSpecificOrigins";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddCors(options => {
                options.AddPolicy(AllowSpecificOrigins,
                builder => {
                    builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
                });
            });

            services.AddHangfire(config => {
                config.UseMemoryStorage();
            });

            services.AddControllers();

            services.AddSignalR()
                .AddJsonProtocol(config => {
                    config.PayloadSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddSingleton<Controller>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(AllowSpecificOrigins);

            app.UseAuthorization();

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapHub<GameHub>("/gameHub");
            });
        }
    }
}
