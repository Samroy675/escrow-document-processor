namespace EscrowDocumentProcessor.Models
{
    public class ParsedOcrDocument
    {
        public string FullContent { get; set; } = string.Empty;
        public List<PageData> Pages { get; set; } = new();
        public List<ParagraphData> Paragraphs { get; set; } = new();
    }

    public class PageData
    {
        public int PageNumber { get; set; }
        public List<string> Lines { get; set; } = new();
    }

    public class ParagraphData
    {
        public string Content { get; set; } = string.Empty;
        public int Offset { get; set; }
        public int Length { get; set; }
    }
}
