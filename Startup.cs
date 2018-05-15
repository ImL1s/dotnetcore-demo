using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dotnetcore_demo.Model;
using dotnetcore_demo.Services;
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
            services.AddMvc();

            # region add single scoped
            services.AddScoped<ISampleScoped, Sample>();
            services.AddTransient<ISampleTransient, Sample>();
            services.AddSingleton<ISampleSingleton, Sample>();

            // Singleton 也可以用以下方法註冊
            // services.AddSingleton<ISampleSingleton>(new Sample());
            # endregion

            #region use service to add scoped, transient or singleton
            services.AddScoped<SampleService, SampleService>();
            # endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifeTime)
        {
            Program.Output("Configure - Calling");
            app.UseMvcWithDefaultRoute();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            registerAppLifeTime(appLifeTime);

            # region Example is here.
            // middlewareExample1(app);
            // middlewareExample2(app);

            // 註冊middleware
            // app.UseMiddleware<FirstMiddleware>();

            // 使用 extensions 方式註冊 middleware
            app.UseFirstMiddleware();
            # endregion


            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World! \r\n");
            });

            Program.Output("Configure - Calling");
        }

        public void registerAppLifeTime(IApplicationLifetime appLifeTime)
        {
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
        }

        /*
         * example for app.Run() & app.Use()
         * Use(): Middleware 的註冊方式是在 Startup.cs 的 Configure 對 IApplicationBuilder 使用 Use 方法註冊。
         * Run(): 是 Middleware 的最後一個行為，就是最末端的 Action。
         * 它不像 Use 能串聯其他 Middleware，但 Run 還是能完整的使用 Request 及 Response。
         */
        public void middlewareExample1(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("First Middleware in. \r\n");
                await next.Invoke();
                await context.Response.WriteAsync("First Middleware out. \r\n");
            });

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Second Middleware in. \r\n");

                // 水管阻塞，封包不往後送
                // var condition = false;
                // if (condition)
                // {
                //     await next.Invoke();
                // }
                await next.Invoke();
                await context.Response.WriteAsync("Second Middleware out. \r\n");
            });

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Third Middleware in. \r\n");
                await next.Invoke();
                await context.Response.WriteAsync("Third Middleware out. \r\n");
            });
        }

        /*
         * example for app.Map()
         * Map(): 是能用來處理一些簡單路由的 Middleware，可依照不同的 URL 指向不同的 Run 及註冊不同的 Use。
         */
        public void middlewareExample2(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("First Middleware in. \r\n");
                await next.Invoke();
                await context.Response.WriteAsync("First Middleware out. \r\n");
            });

            app.Map("/second", mapApp =>
            {
                mapApp.Use(async (context, next) =>
                {
                    await context.Response.WriteAsync("Second Middleware in. \r\n");
                    await next.Invoke();
                    await context.Response.WriteAsync("Second Middleware out. \r\n");
                });

                mapApp.Run(async context =>
                {
                    await context.Response.WriteAsync("Second. \r\n");
                });
            });
        }
    }
}
