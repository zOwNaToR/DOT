using Auth.DTOs;
using DataManager.Common.POCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Abstractions
{
    public interface ITokenService
    {
        Task<IEnumerable<Claim>> GetUserClaimsAsync(User user);
        JwtToken GenerateJwtToken(IEnumerable<Claim> claims);
        Task<RefreshToken?> GenerateRefreshTokenAsync(Guid UserId, string JwtTokenId);
    }
}
