using Auth.Abstractions;
using Auth.DTOs.Requests;
using Auth.DTOs.Responses;
using DataManager.Common.POCOs;
using Email.Abstractions;
using Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Auth.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly HttpContext _httpContext;
        private readonly IEmailSender _emailSender;

        public AuthenticationService(UserManager<User> userManager, 
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender
        )
        {
            _userManager = userManager;
            _httpContext = httpContextAccessor!.HttpContext!;
            _emailSender = emailSender;
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
    }
}
