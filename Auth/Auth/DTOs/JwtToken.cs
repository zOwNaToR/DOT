namespace Auth.DTOs
{
    public record JwtToken(string Id, string Token, DateTime ExpireDate);
}
