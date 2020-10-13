using Features.Payments.Services;
using Features.Payments.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.IO;

namespace PaymentGateway.Api
{
    public class Startup
    {
        public const string DefaultApiVersion = "1.0";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHealthChecks();

            services
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "VV";
                    options.SubstituteApiVersionInUrl = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = ApiVersion.Parse(DefaultApiVersion);
                })
                .AddApiVersioning(options => options.ReportApiVersions = true);

            services
                .AddTransient<IBankService, BankService>()
                .AddTransient<IPaymentStoreWriter, PaymentStore>()
                .AddTransient<IPaymentStoreReader, PaymentStore>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(DefaultApiVersion, new OpenApiInfo
                {
                    Title = "Payment Gateway API",
                    Version = DefaultApiVersion,
                    Description = "An API to enable merchants to offer a way for their shoppers to pay for their product.",
                    Contact = new OpenApiContact { Email = "masoud.work@gmail.com" }
                });

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PaymentGateway.Api.xml"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider vdp)
        {
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
            });

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in vdp.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                       $"/swagger/{description.GroupName}/swagger.json",
                       $"Payment Gateway API {description.GroupName}");
                }
            });
        }
    }
}
