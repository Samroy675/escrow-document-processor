using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace EscrowDocumentProcessor.Services
{
    public class AzureOpenAiService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly string _deploymentName;

        public AzureOpenAiService(
            string endpoint,
            string apiKey,
            string deploymentName)
        {
            _endpoint = endpoint;
            _apiKey = apiKey;
            _deploymentName = deploymentName;
        }

        public async Task<string> ExtractStructuredDataAsync(string prompt)
        {
            var client = new AzureOpenAIClient(
                new Uri(_endpoint),
                new AzureKeyCredential(_apiKey));

            var chatClient = client.GetChatClient(_deploymentName);

            var response = await chatClient.CompleteChatAsync(
                new List<ChatMessage>
                {
                new SystemChatMessage(
                    "You are an intelligent document extraction assistant."),
                new UserChatMessage(prompt)
                });

            return response.Value.Content[0].Text;
        }
    }
}

