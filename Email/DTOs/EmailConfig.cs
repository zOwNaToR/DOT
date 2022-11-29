namespace Email.DTOs
{
    public class EmailConfig
    {
        public string DefaultFrom { get; set; } = "";
        public string DefaultPassword { get; set; } = "";
        public string Host { get; set; } = "";
        public int Port { get; set; }
    }
}
