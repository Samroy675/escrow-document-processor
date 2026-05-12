namespace EscrowDocumentProcessor.Prompts
{
    public static class EscrowExtractionPromptTemplate
    {
        public const string Template = """
        Analyze the OCR content from the provided document.

        Extract the required fields and return ONLY valid JSON.

        Required fields:
        - Escrow Reference Number
        - Agreement Date
        - Agreement Type
        - Jurisdiction
        - Escrow Agent Name
        - Agent License Number
        - Agent Contact Email
        - Depositor Full Name
        - Depositor ID
        - Depositor Address
        - Beneficiary Full Name
        - Beneficiary Tax ID
        - Beneficiary Bank Account
        - Total Escrow Amount
        - Initial Deposit Amount
        - Escrow Service Fee
        - Disbursement Currency
        - Release Trigger Event
        - Escrow Expiry Date
        - AML Verification Status

        Return JSON in this exact format:

        [
          {
            "fieldName": "",
            "extractedValue": "",
            "pageNumber": 0,
            "paragraphReference": "",
            "lineReference": ""
          }
        ]

        Rules:
        - Return only valid JSON.
        - Do not explain anything.
        - Do not hallucinate values.
        - If field not found, return empty string.
        - Include correct page number.
        - Include paragraph reference where available.
        - Include line reference where available.

        OCR CONTENT:
        {{OCR_CONTENT}}
        """;
    }

}


