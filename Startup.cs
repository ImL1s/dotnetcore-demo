using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dotnetcore_demo.Model;
using dotnetcore_demo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

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

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            # region Static files example is here.
            // UseDefaultFiles 必須註冊在 UseStaticFiles 之前
            // 如果先註冊 UseStaticFiles，當 URL 是 / 時，UseStaticFiles 找不到該檔案，就會直接回傳找不到；所以就沒有機會進到 UseDefaultFiles
            app.UseDefaultFiles();

            // localhost:xxxx/ -> ~/wwwroot/ (UseStaticFiles 預設啟用靜態檔案的目錄是 wwwroot)
            app.UseStaticFiles();

            // localhost:xxxx/third-party -> ~/node_modules/
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                     Path.Combine(env.ContentRootPath, @"node_modules")
                 ),
                RequestPath = new PathString("/third-party")
            });

            // 使用 file server 讓client可以看得到目錄
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, @"bin")
                ),
                RequestPath = new PathString("/StaticFiles"),
                EnableDirectoryBrowsing = true
            });
            # endregion

            # region Routing example is here.
            // [MVC Routing]
            app.UseMvc(routes1 =>
            {
                routes1.MapRoute(
                    name: "about",
                    template: "about",
                    defaults: new { controller = "Home", action = "About" }
                );

                routes1.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );

                // 跟上面設定的 default 效果一樣
                //routes1.MapRoute(
                //    name: "default",
                //    template: "{controller}/{action}/{id?}",
                //    defaults: new { controller = "Home", action = "Index" }
                //);
            });

            // [Routing]
            var defaultRouteHandler = new RouteHandler(context =>
            {
                var routeValues = context.GetRouteData().Values;
                return context.Response.WriteAsync($"Route values:{string.Join(", ", routeValues)}");
            });

            var routeBuilder = new RouteBuilder(app, defaultRouteHandler);
            // first:xxx 代表將 first後面的xxx賦值給 first 這個route value後面
            routeBuilder.MapRoute("default", "{first:regex(^(default|home)$)}/{second?}");
            routeBuilder.MapGet("user/{name}", context =>
            {
                var name = context.GetRouteValue("name");
                return context.Response.WriteAsync($"Get user. name: {name}");
            });
            routeBuilder.MapPost("user/{name}", context =>
            {
                var name = context.GetRouteValue("name");
                return context.Response.WriteAsync($"Create user. name: {name}");
            });

            var routes = routeBuilder.Build();
            app.UseRouter(routes);
            # endregion

            // app.UseMvcWithDefaultRoute();

            registerAppLifeTime(appLifeTime);

            # region Middleware example is here.
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
