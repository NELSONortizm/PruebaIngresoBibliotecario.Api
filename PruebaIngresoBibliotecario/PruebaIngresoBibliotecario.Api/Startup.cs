using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PruebaIngresoBibliotecario.Api.Services;



namespace PruebaIngresoBibliotecario.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            Trace.Indent();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuración de Swagger

            services.AddSwaggerDocument();

            // Configuración de DbContext en memoria

            services.AddDbContext<Infrastructure.PersistenceContext>(opt =>
            {
                opt.UseInMemoryDatabase("PruebaIngreso");
            });

            // Registrar servicios
            services.AddScoped<IPrestamoService, PrestamoService>();

            // Configuración de controladores

            services.AddControllers(mvcOpts =>
            {
            });

        }


        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Redirección HTTPS
            app.UseHttpsRedirection();

            // Configuración de rutas
            app.UseRouting();

            // Mapear endpoints de controladores
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Configuración de Swagger
            app.UseOpenApi();
            app.UseSwaggerUi3();

        }
    }
}
