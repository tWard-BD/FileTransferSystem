using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FileTransferSystem.Services.Services;
using FileTransferSystem.Services.Services.Contracts;
using FileTransferSystem.Common.Configuration;


namespace FileTransferSystem.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddScoped<IFileProcessingService, FileProcessingService>();
            services.AddScoped<IMoveItApiClient, MoveItApiClient>();
            services.AddHttpClient<IMoveItApiClient, MoveItApiClient>(c => 
            {
                c.BaseAddress = new Uri("https://mobile-1.moveitcloud.com/api/v1/");
            });
            services.AddSingleton<HttpClient>();
            var userConfiguration = Configuration.GetSection("UserConfiguration");
            services.Configure<UserConfiguration>(userConfiguration);
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
               
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
