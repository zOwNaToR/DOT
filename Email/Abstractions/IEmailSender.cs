namespace Email.Abstractions
{
    public interface IEmailSender
    {
        Task SendAsync(string from, string to, string subject, string html);
        Task SendAsync(string from, IEnumerable<string> to, string subject, string html);
    }
}
