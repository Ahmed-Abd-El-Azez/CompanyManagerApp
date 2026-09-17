namespace CompanyMaster
{
    public class ChatRequestDto
    {
        public int session_id { get; set; }
        public string message { get; set; } = string.Empty;
    }

    public class ChatResponseDto
    {
        public string Output { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}