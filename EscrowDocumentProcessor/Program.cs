using EscrowDocumentProcessor.Extensions;
using EscrowDocumentProcessor.Prompts;
using EscrowDocumentProcessor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.AddApplicationServices(configuration);

var serviceProvider = services.BuildServiceProvider();

var documentService = serviceProvider.GetRequiredService<DocumentIntelligenceService>();

var pdfPath = configuration["DocumentSettings:PdfPath"];

if (string.IsNullOrWhiteSpace(pdfPath))
{
    Console.WriteLine("PDF path is not configured.");
    return;
}

if (!File.Exists(pdfPath))
{
    Console.WriteLine($"PDF file not found: {pdfPath}");
    return;
}

using var pdfStream = File.OpenRead(pdfPath);

var ocrJsonPath = Path.Combine(
    Path.GetDirectoryName(pdfPath)!,
    $"{Path.GetFileNameWithoutExtension(pdfPath)}.ocr.json");

string ocrJsonContent;

if (File.Exists(ocrJsonPath))
{
    Console.WriteLine("Existing OCR JSON found. Skipping Document Intelligence call...");
    ocrJsonContent = await File.ReadAllTextAsync(ocrJsonPath);
}
else
{
    Console.WriteLine("No OCR JSON found. Invoking Azure Document Intelligence...");

    var analyzeResult = await documentService.AnalyzeDocumentAsync(pdfStream);

    ocrJsonContent = JsonConvert.SerializeObject(
        analyzeResult,
        Formatting.Indented);

    await File.WriteAllTextAsync(
        ocrJsonPath,
        ocrJsonContent);
}

//parse OCR JSON into structured model
var ocrParserService = serviceProvider.GetRequiredService<OcrParserService>();
var parsedOcrDocument = ocrParserService.Parse(ocrJsonContent);


//build prompt for LLM
var promptBuilder = serviceProvider.GetRequiredService<EscrowExtractionPromptBuilder>();

var prompt = promptBuilder.BuildPrompt(parsedOcrDocument);


//invoke LLM to extract structured data
var openAiService = serviceProvider.GetRequiredService<AzureOpenAiService>();

Console.WriteLine("Configuration loaded successfully.");
Console.WriteLine("PDF loaded successfully.");
Console.WriteLine();

Console.WriteLine($"File Name: {Path.GetFileName(pdfPath)}");
Console.WriteLine($"File Size: {pdfStream.Length} bytes");
Console.WriteLine();

Console.WriteLine($"OCR JSON Path: {ocrJsonPath}");
Console.WriteLine("OCR JSON parsed successfully.");
Console.WriteLine();

Console.WriteLine($"Total Pages: {parsedOcrDocument.Pages.Count}");
Console.WriteLine($"Total Paragraphs: {parsedOcrDocument.Paragraphs.Count}");
Console.WriteLine();

Console.WriteLine("Prompt built successfully.");
Console.WriteLine($"Prompt Length: {prompt.Length} characters");
Console.WriteLine();


Console.WriteLine("Invoking Azure OpenAI....");
var extractionResponse = await openAiService.ExtractStructuredDataAsync(prompt);
Console.WriteLine("Azure OpenAI response received.");
Console.WriteLine();

//save output JSON to file
var outputJsonPath = Path.Combine(Path.GetDirectoryName(pdfPath)!,
    $"{Path.GetFileNameWithoutExtension(pdfPath)}.output.json");
try
{
    var parsedJson = JToken.Parse(extractionResponse);
    await File.WriteAllTextAsync(outputJsonPath, parsedJson.ToString(Formatting.Indented));
    Console.WriteLine($"Output JSON saved: {outputJsonPath}");
}
catch (Exception ex)
{
    throw new Exception("Failed to parse LLM response as JSON. Response content: " + extractionResponse, ex);
}

//save parsed version for debugging
var parsedJsonPath = Path.Combine(
    Path.GetDirectoryName(pdfPath)!,

    $"{Path.GetFileNameWithoutExtension(pdfPath)}.parsed.json");

await File.WriteAllTextAsync(
    parsedJsonPath,

    JsonConvert.SerializeObject(parsedOcrDocument, Formatting.Indented));

//save prompt to file for debugging
var promptPath = Path.Combine(
    Path.GetDirectoryName(pdfPath)!,
    "EscrowExtractionPrompt.txt");

await File.WriteAllTextAsync(promptPath, prompt);