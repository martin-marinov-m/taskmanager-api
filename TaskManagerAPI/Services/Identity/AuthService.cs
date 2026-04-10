using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPI.Models.Identity;

namespace TaskManagerAPI.Services.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<TaskManagerUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<TaskManagerUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }
        public async Task<string> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnauthorizedAccessException("Invalid email or password");

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var jwtKey = _config["JWT:Key"] ?? throw new KeyNotFoundException("JWT key not Found");
            var jwtIssuer = _config["JWT:Issuer"] ?? throw new KeyNotFoundException("JWT issuer not Found");
            var jwtAudience = _config["JWT:Audience"] ?? throw new KeyNotFoundException("JWT audience not Found");
            var jwtTokenExpirationInHours = _config["JWT:TokenExpirationInHours"] ?? throw new KeyNotFoundException("JWT tokenExpirationInHours not Found");

            int tokenExpirationInHours;

            if (!int.TryParse(jwtTokenExpirationInHours, out tokenExpirationInHours))
                throw new InvalidOperationException("JWT TokenExpirationInHours is invalid");


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(tokenExpirationInHours),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
