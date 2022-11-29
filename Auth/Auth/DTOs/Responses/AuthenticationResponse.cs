namespace Auth.DTOs.Responses
{
    public class AuthenticationResponse : BaseResponse
    {
        public string Token { get; set; } = "";
        public Guid RefreshToken { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool RefreshTokenHidden { get; set; }

        public string UserName { get; set; } = "";
        public List<string> Roles { get; set; } = new List<string>();

        public AuthenticationResponse() { }
        public AuthenticationResponse(bool success) : base(success) { }
        public AuthenticationResponse(bool success, List<string> errors) : base(success, errors) { }

        public void HideRefreshToken()
        {
            RefreshToken = Guid.Empty;
            RefreshTokenHidden = true;
        }

    }
}
