namespace Auth.DTOs
{
    public class TokenConfig
    {
        public int TokenExpiresIn { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
        public string Audience { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string SecretKey { get; set; } = "";
    }
}
