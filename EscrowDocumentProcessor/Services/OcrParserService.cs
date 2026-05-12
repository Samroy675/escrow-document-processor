using EscrowDocumentProcessor.Models;
using Newtonsoft.Json.Linq;

namespace EscrowDocumentProcessor.Services
{
    public class OcrParserService
    {
        public ParsedOcrDocument Parse(string ocrJsonContent)
        {
            var json = JObject.Parse(ocrJsonContent);

            var parsedDocument = new ParsedOcrDocument
            {
                FullContent = json["Content"]?.ToString() ?? string.Empty
            };

            var pages = json["Pages"];

            if (pages != null)
            {
                foreach (var page in pages)
                {
                    var pageData = new PageData
                    {
                        PageNumber = page["PageNumber"]?.Value<int>() ?? 0
                    };

                    var lines = page["Lines"];

                    if (lines != null)
                    {
                        foreach (var line in lines)
                        {
                            pageData.Lines.Add(
                                line["Content"]?.ToString() ?? string.Empty);
                        }
                    }

                    parsedDocument.Pages.Add(pageData);
                }
            }

            var paragraphs = json["Paragraphs"];

            if (paragraphs != null)
            {
                foreach (var paragraph in paragraphs)
                {
                    var span = paragraph["Spans"]?.First;

                    parsedDocument.Paragraphs.Add(new ParagraphData
                    {
                        Content = paragraph["Content"]?.ToString() ?? string.Empty,
                        Offset = span?["Offset"]?.Value<int>() ?? 0,
                        Length = span?["Length"]?.Value<int>() ?? 0
                    });
                }
            }

            return parsedDocument;
        }
    }
}

