using Auth.Abstractions;
using Auth.DTOs.Requests;
using Auth.DTOs.Responses;
using DataManager.Common.POCOs;
using Email.Abstractions;
using Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly ITokenService _tokerService;

        public AuthenticationService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender,
            ITokenService tokerService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContext = httpContextAccessor!.HttpContext!;
            _emailSender = emailSender;
            _tokerService = tokerService;
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

        private async Task<AuthenticationResponse> AuthenticateUser(User user)
        {
            var claims = await _tokerService.GetUserClaimsAsync(user);
            var jwtToken = _tokerService.GenerateJwtToken(claims);
            var refreshToken = await _tokerService.GenerateRefreshTokenAsync(user.Id, jwtToken.Id);

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
    }
}
