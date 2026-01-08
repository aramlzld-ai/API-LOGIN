using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace LoginApi
{
    public class SecurityTools
    {
        public string HashearPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        public string GenerarToken(string usuario, string rol, string key)
        {
            var claims = new[]
            {
        new System.Security.Claims.Claim("usuario", usuario),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, rol)
    };

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
