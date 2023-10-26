using AutoMapper;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Occtoo.Onboarding.Sdk;
using Occtoo.Provider.Centra;
using Occtoo.Provider.Centra.Mappers;
using Occtoo.Provider.Centra.Models;
using Occtoo.Provider.Centra.Services;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Occtoo.Provider.Centra
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(logger => { logger.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); });
            builder.Services.AddHttpClient("Default").AddPolicyHandler(GetRetryPolicy());
            ConfigureServices(builder.Services);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(ParseSettingsFromEnvironmentVariables());
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IOcctooExporter, OcctooExporter>();
            services.AddScoped<ICentraService, CentraService>();
            services.AddSingleton<IOnboardingServiceClient>(new OnboardingServiceClient(
                    Environment.GetEnvironmentVariable("OcctooDataProviderId"),
                    Environment.GetEnvironmentVariable("OcctooDataProviderSecret")
                ));
            services.AddSingleton(new TableServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddHttpClient<ICentraService>()
                .AddPolicyHandler(GetRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(result => !result.IsSuccessStatusCode)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public static AppSettings ParseSettingsFromEnvironmentVariables()
        {
            var settings = new AppSettings
            {
                AzureWebJobsStorage = Environment.GetEnvironmentVariable(nameof(AppSettings.AzureWebJobsStorage)),
                DataProviderId = Environment.GetEnvironmentVariable(nameof(AppSettings.DataProviderId)),
                DataProviderSecret = Environment.GetEnvironmentVariable(nameof(AppSettings.DataProviderSecret)),
                PriceSource = Environment.GetEnvironmentVariable(nameof(AppSettings.PriceSource)),
                ProductSource = Environment.GetEnvironmentVariable(nameof(AppSettings.ProductSource)),
                StockSource = Environment.GetEnvironmentVariable(nameof(AppSettings.StockSource)),
                VariantSource = Environment.GetEnvironmentVariable(nameof(AppSettings.VariantSource)),
            };
            return settings;
        }
    }
}
