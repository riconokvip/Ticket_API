using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ticket.API.JwtAuthentications
{
    public interface IJwtHelper
    {
        /// <summary>
        /// Xác thực token
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns></returns>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        /// <summary>
        /// Tạo token mới
        /// </summary>
        /// <param name="user">Người dùng</param>
        /// <param name="roles">Danh sách quyền</param>
        /// <returns></returns>
        JwtResponse GenerateToken(UserEntities user, List<string> roles);
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly JwtTokenConfig _config;

        public JwtHelper(IOptions<JwtTokenConfig> config)
        {
            _config = config.Value;
        }

        public JwtResponse GenerateToken(UserEntities user, List<string> roles)
        {
            if (user != null && !user.IsDeleted)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Id),
                    new Claim(ClaimTypes.NameIdentifier, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var accessTokenExp = DateTime.UtcNow.AddMinutes(_config.TokenValidityInMinutes);
                var token = this.CreateToken(authClaims, accessTokenExp);
                var refreshToken = this.GenerateRefreshToken();

                return new JwtResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    AccessTokenExpriesAt = accessTokenExp,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.UtcNow.AddDays(_config.RefreshTokenValidityInDays)
                };
            }
            return null;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret)),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims, DateTime exp)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
            return new JwtSecurityToken(
                issuer: _config.ValidIssuer,
                audience: _config.ValidAudience,
                expires: exp,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
