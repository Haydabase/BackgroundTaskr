using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DemoApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "DemoApp", Version = "v1"}); });

            services.AddBackgroundTaskr(options =>
                {
                    options.EnrichCreate = (activity, eventName, rawObject) =>
                    {
                        switch (eventName)
                        {
                            case "OnStartActivity" when rawObject is string taskName:
                            {
                                activity.SetTag("custom_task_name", taskName);
                                break;
                            }
                            case "OnStopActivity" when rawObject is string taskName:
                            {
                                activity.SetTag("total_milliseconds", activity.Duration.TotalMilliseconds);
                                break;
                            }
                        }
                    };
                    options.EnrichRun = (activity, eventName, rawObject) =>
                    {
                        switch (eventName)
                        {
                            case "OnStartActivity" when rawObject is string taskName:
                            {
                                activity.SetTag("custom_task_name", taskName);
                                break;
                            }
                            case "OnException" when rawObject is Exception exception:
                            {
                                activity.SetTag("stack_trace", exception.StackTrace);
                                break;
                            }
                            case "OnStopActivity" when rawObject is string taskName:
                            {
                                activity.SetTag("total_milliseconds", activity.Duration.TotalMilliseconds);
                                break;
                            }
                        }
                    };
                })
                .UsingSlackErrorNotifications();
            
            services.AddOpenTelemetryTracing(
                x => x
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BackgroundTaskr.DemoApp"))
                    .AddBackgroundTaskrInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoApp v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}