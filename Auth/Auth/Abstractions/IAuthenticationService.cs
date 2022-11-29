using Auth.DTOs.Requests;
using Auth.DTOs.Responses;

namespace Auth.Abstractions
{
    public interface IAuthenticationService
    {
        Task<BaseResponse> RegisterAsync(RegisterUserRequest request);
        Task<AuthenticationResponse> LoginAsync(string email, string password);
        //Task<AuthenticationResponse> RefreshTokenAsync(string token, string refreshToken);
        //Task<AuthenticationResponse> RevokeRefreshTokenAsync(string token);
        //Task<SendResetPasswordLinkResponse> SendPasswordResetLinkAsync(string email);
        //Task<BaseResponse> ResetPasswordAsync(Guid userId, string password, string resetPasswordToken);
        //Task<BaseResponse> ConfirmEmailAsync(Guid userId, string confirmEmailToken);
    }
}
