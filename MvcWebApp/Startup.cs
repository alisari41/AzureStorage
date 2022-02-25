using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorageLibrary;
using AzureStorageLibrary.Services;
using MvcWebApp.Hubs;

namespace MvcWebApp
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

            //ConnectionString
            ConnectionStrings.AzureStorageConnectionString = Configuration.GetSection("AzureConnectionStrings")
                ["StorageCloudStr"];//appsetting.json i�indeki Azure cloud de�eri okucam 
            //�lk Ba�ta ekleme,silme, g�ncelleme vs. i�lemlerini Local�de yaparsam daha iyi olur ��nk� deneme i�lemlerinde s�rekli azure cloud storage kullan�rsam fiyat s�rekli artar.
            //E�er StorageConStr olursa Local  Storage Primary Connection String Adresinde i�lem yapar
            //E�er StorageCloudStr olursa Azure cloud  Storage Primary Connection String Adresinde i�lem yapar
            services.AddScoped(typeof(INoSqlStorage<>), typeof(TableStorage<>));
            //BlobStorage dab bir nesne �rne�i al
            services.AddSingleton<IBlobStorage, BlobStorage>();
            //AddScoped da her seferinde al�n�r AddSingleton da uygulama aya�a kalkt���nda sadece bir kere al�n�r.



            services.AddControllersWithViews();


            //SignalR ekleme
            services.AddSignalR();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //SignalR hub ekleme
                endpoints.MapHub<NotificationHub>("/notificationHub");


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
