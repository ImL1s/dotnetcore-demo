using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace dotnetcore_demo
{
    public class Startup
    {
        public Startup()
        {
            Program.Output("Startup Constructor - Called");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Program.Output("ConfigureServices");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifeTime)
        {
            Program.Output("Configure - Calling");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            appLifeTime.ApplicationStarted.Register(() =>
            {
                Program.Output("ApplicationStarted - Started");
            });

            appLifeTime.ApplicationStopping.Register(() =>
            {
                Program.Output("ApplicationStarted - Stopping");
            });

            appLifeTime.ApplicationStopped.Register(() =>
            {
                Program.Output("ApplicationStarted - Stopped");
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });

            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(5 * 1000);
                Program.Output("Trigger stop WebHost");
                appLifeTime.StopApplication();
            })).Start();

            Program.Output("Configure - Calling");
        }
    }
}
