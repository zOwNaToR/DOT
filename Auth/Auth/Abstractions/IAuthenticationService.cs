using Auth.DTOs.Requests;
using Auth.DTOs.Responses;

namespace Auth.Abstractions
{
    public interface IAuthenticationService
    {
        Task<BaseResponse> RegisterAsync(RegisterUserRequest request);
        Task<BaseResponse> ConfirmEmailAsync(Guid userId, string confirmEmailToken);
        Task<AuthenticationResponse> LoginAsync(string email, string password);
        Task<AuthenticationResponse> RevokeRefreshTokenAsync(Guid refreshTtoken);
        Task<AuthenticationResponse> RefreshTokenAsync(string token, Guid refreshToken);
        //Task<SendResetPasswordLinkResponse> SendPasswordResetLinkAsync(string email);
        //Task<BaseResponse> ResetPasswordAsync(Guid userId, string password, string resetPasswordToken);
    }
}
