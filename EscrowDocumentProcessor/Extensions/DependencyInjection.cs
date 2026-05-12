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
                    configuration["AzureDocumentIntelligence:Endpoint"] ?? throw new InvalidOperationException("Azure Document Intelligence endpoint is not configured."),
                    configuration["AzureDocumentIntelligence:ApiKey"] ?? throw new InvalidOperationException("Azure Document Intelligence API key is not configured.")));

            services.AddSingleton<OcrParserService>();

            services.AddSingleton<EscrowExtractionPromptBuilder>();

            services.AddSingleton<AzureOpenAiService>(_ =>
                    new AzureOpenAiService(
                        configuration["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("Azure OpenAI endpoint is not configured."),
                        configuration["AzureOpenAI:ApiKey"] ?? throw new InvalidOperationException("Azure OpenAI API key is not configured."),
                        configuration["AzureOpenAI:DeploymentName"] ?? throw new InvalidOperationException("Azure OpenAI deployment name is not configured.")));

            return services;
        }
    }
}

