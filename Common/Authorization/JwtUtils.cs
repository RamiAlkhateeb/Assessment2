
using Clincs.Common.Helpers;
using Common.Models.Database.API;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace Common.Authorization
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(User user);
        public TokenUser? ValidateJwtToken(string token);
    }

    public class JwtUtils : IJwtUtils
    {
        private readonly AppSettings _appSettings;

        public JwtUtils(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string GenerateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            if (String.IsNullOrEmpty(user.AadObjectId))
                user.AadObjectId = "";
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) , 
                    new Claim("role", user.Role.ToString() ) , new Claim("AadObjectId" , user.AadObjectId) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public TokenUser? ValidateJwtToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var tokenUser = new TokenUser();
                tokenUser.userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                tokenUser.AadObjectId = jwtToken.Claims.First(x => x.Type == "AadObjectId").Value;
                tokenUser.Role = (Role)Enum.Parse(typeof(Role), jwtToken.Claims.First(x => x.Type == "role").Value);
                // return user id from JWT token if validation successful
                return tokenUser;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }

    public class TokenUser
    {
        public int userId { get; set; }
        public string AadObjectId { get; set; }
        public Role Role { get; set; }
    }
}