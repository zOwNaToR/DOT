using Auth.Abstractions;
using Auth.DTOs.Requests;
using Auth.DTOs.Responses;
using DataManager.Common.Abstractions;
using DataManager.Common.POCOs;
using Email.Abstractions;
using Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;

namespace Auth.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly HttpContext _httpContext;
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender,
            ITokenService tokerService,
            IUnitOfWork unitOfWork
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContext = httpContextAccessor!.HttpContext!;
            _emailSender = emailSender;
            _tokenService = tokerService;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> RegisterAsync(RegisterUserRequest request)
        {
            var response = new BaseResponse();
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                response.Errors.Add("User with this email address already exists");
                return response;
            }

            var newUser = request.MapToPoco();

            var createdUser = await _userManager.CreateAsync(newUser, request.Password);
            if (!createdUser.Succeeded)
            {
                response.Errors.AddRange(createdUser.Errors.Select(x => x.Description));
                return response;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmEmailLink = $"{_httpContext.Request.GetClientBaseUrl()}/confirm-email?token={WebUtility.UrlEncode(token)}&userId={newUser.Id}";

            await _emailSender.SendAsync("", newUser.Email, "Confirm Email .NET Auth", $"<a href=\"{confirmEmailLink}\">Click here to confirm your email</a>");

            response.Success = true;
            return response;
        }

        public async Task<AuthenticationResponse> LoginAsync(string email, string password)
        {
            var errorResponse = new AuthenticationResponse();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                errorResponse.Errors.Add("Wrong email or password");
                return errorResponse;
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
            {
                errorResponse.Errors.Add("Wrong email or password");
                return errorResponse;
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, true, false);
            if (!result.Succeeded)
            {
                errorResponse.Errors.Add("Wrong email or password");
                return errorResponse;
            }

            return await AuthenticateUser(user);
        }

        public async Task<BaseResponse> ConfirmEmailAsync(Guid userId, string confirmEmailToken)
        {
            var response = new BaseResponse();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                response.Errors.Add("User not found");
                return response;
            }

            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailToken);
            if (!result.Succeeded)
            {
                response.Errors.AddRange(result.Errors.Select(x => x.Description));
                return response;
            }

            await _emailSender.SendAsync("", user.Email, "Email confirmed .NET Auth", $"Your email has been confirmed.");

            return new BaseResponse(true);
        }

        public async Task<AuthenticationResponse> RevokeRefreshTokenAsync(Guid refreshToken)
        {
            var errorResponse = new AuthenticationResponse();

            var dbRefreshToken = await _unitOfWork.RefreshTokenRepository.GetAsync(x => x.Id == refreshToken);
            if (dbRefreshToken == null)
            {
                errorResponse.Errors.Add("Refresh token not found");
                return errorResponse;
            }
            if (dbRefreshToken.Invalidated)
            {
                errorResponse.Errors.Add("Refresh token is not active");
                return errorResponse;
            }

            dbRefreshToken.Invalidated = true;
            await _unitOfWork.SaveAsync();

            return new AuthenticationResponse(true);
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(string token, Guid refreshToken)
        {
            var errorResponse = new AuthenticationResponse();

            if (!string.IsNullOrEmpty(token) && !_tokenService.IsJwtTokenValid(token))
            {
                errorResponse.Errors.Add("Invalid token");
                return errorResponse;
            }

            var dbRefreshToken = await _unitOfWork.RefreshTokenRepository.GetAsync(x => x.Id == refreshToken);
            if (dbRefreshToken == null)
            {
                errorResponse.Errors.Add("Refresh token not found");
                return errorResponse;
            }
            if (dbRefreshToken.Invalidated)
            {
                await InvalidateAllUserRefreshTokensAsync(dbRefreshToken.UserId);

                errorResponse.Errors.Add("Refresh token has been invalidated");
                return errorResponse;
            }
            if (dbRefreshToken.ExpireDate < DateTime.UtcNow)
            {
                errorResponse.Errors.Add("Refresh token expired");
                return errorResponse;
            }
            if (dbRefreshToken.Used)
            {
                errorResponse.Errors.Add("Refresh token has been used");
                return errorResponse;
            }

            dbRefreshToken.Used = true;
            await _unitOfWork.SaveAsync();

            var user = await _userManager.FindByIdAsync(dbRefreshToken.UserId.ToString());
            return await AuthenticateUser(user);
        }

        private async Task<AuthenticationResponse> AuthenticateUser(User user)
        {
            var claims = await _tokenService.GetUserClaimsAsync(user);
            var jwtToken = _tokenService.GenerateJwtToken(claims);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id, jwtToken.Id);

            if (refreshToken is null)
            {
                return new AuthenticationResponse(false, new List<string> { "User not found" });
            }

            return new AuthenticationResponse
            {
                Success = true,
                Token = jwtToken.Token,
                ExpireDate = jwtToken.ExpireDate,
                RefreshToken = refreshToken.Id,
                UserName = user.UserName,
                Roles = claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList(),
            };
        }

        private async Task InvalidateAllUserRefreshTokensAsync(Guid userId)
        {
            var dbRefreshTokens = await _unitOfWork.RefreshTokenRepository.SearchAsync(x => x.UserId == userId && !x.Invalidated);
            foreach (var dbRefreshToken in dbRefreshTokens)
            {
                dbRefreshToken.Invalidated = true;
            }

            await _unitOfWork.SaveAsync();
        }
    }
}
