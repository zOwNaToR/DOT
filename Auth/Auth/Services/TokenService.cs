﻿using Auth.Abstractions;
using Auth.DTOs;
using Auth.DTOs.Responses;
using DataManager.Common;
using DataManager.Common.Abstractions;
using DataManager.Common.POCOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenConfig _tokenConfig;

        public TokenService(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IUnitOfWork unitOfWork,
            IOptions<TokenConfig> tokenConfig
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _tokenConfig = tokenConfig.Value;
        }

        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Id", user.Id.ToString()),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Take user's roles and map them to Claims. For each role it takes all realted claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) continue;

                claims.Add(new Claim(ClaimTypes.Role, roleName));

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                var filteredRoleClaims = roleClaims.Where(roleClaim => !claims.Contains(roleClaim));

                claims.AddRange(filteredRoleClaims);
            }

            return claims;
        }

        public JwtToken GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfig.SecretKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenConfig.TokenExpiresIn),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Audience = _tokenConfig.Audience,
                Issuer = _tokenConfig.Issuer
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwtTokenString = tokenHandler.WriteToken(securityToken);

            return new JwtToken(securityToken.Id, jwtTokenString, securityToken.ValidTo);
        }

        public async Task<RefreshToken?> GenerateRefreshTokenAsync(Guid UserId, string JwtTokenId)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(x => x.Id == UserId, "RefreshTokens");
            if (user is null) return null;
            
            var utcNow = DateTime.UtcNow;
            var expireDateRefreshToken = utcNow.AddDays(_tokenConfig.RefreshTokenExpiresIn);
            var refreshToken = new RefreshToken()
            {
                JwtId = JwtTokenId,
                UserId = UserId,
                CreationDate = utcNow,
                ExpireDate = expireDateRefreshToken,
            };

            user.RefreshTokens.Add(refreshToken);
            await _unitOfWork.SaveAsync();

            return refreshToken;
        }
    }
}
