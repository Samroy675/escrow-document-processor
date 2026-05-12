using Azure;
using Azure.AI.DocumentIntelligence;

namespace EscrowDocumentProcessor.Services
{
    public class DocumentIntelligenceService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;

        public DocumentIntelligenceService(string endpoint, string apiKey)
        {
            _endpoint = endpoint;
            _apiKey = apiKey;
        }

        public async Task<AnalyzeResult> AnalyzeDocumentAsync(Stream pdfStream)
        {
            var client = new DocumentIntelligenceClient(
                new Uri(_endpoint),
                new AzureKeyCredential(_apiKey));

            var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed,
              "prebuilt-layout",
               BinaryData.FromStream(pdfStream));

            return operation.Value;
        }
    }
}
