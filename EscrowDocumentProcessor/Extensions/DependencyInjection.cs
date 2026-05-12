using EscrowDocumentProcessor.Prompts;
using EscrowDocumentProcessor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EscrowDocumentProcessor.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<DocumentIntelligenceService>(_ =>
                new DocumentIntelligenceService(
                    configuration["AzureDocumentIntelligence:Endpoint"]!,
                    configuration["AzureDocumentIntelligence:ApiKey"]!));

            services.AddSingleton<OcrParserService>();

            services.AddSingleton<EscrowExtractionPromptBuilder>();

            services.AddSingleton<AzureOpenAiService>(_ =>
                    new AzureOpenAiService(
                        configuration["AzureOpenAI:Endpoint"]!,
                        configuration["AzureOpenAI:ApiKey"]!,
                        configuration["AzureOpenAI:DeploymentName"]!));

            return services;
        }
    }
}

