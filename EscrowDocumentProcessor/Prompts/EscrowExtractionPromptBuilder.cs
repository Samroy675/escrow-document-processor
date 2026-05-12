using EscrowDocumentProcessor.Models;

namespace EscrowDocumentProcessor.Prompts
{
    public class EscrowExtractionPromptBuilder
    {
        public string BuildPrompt(ParsedOcrDocument parsedDocument)
        {
            return EscrowExtractionPromptTemplate.Template.Replace("{{OCR_CONTENT}}", parsedDocument.FullContent);
        }
    }
}

