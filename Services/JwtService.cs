using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace MyBlogs.Services;


public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtAccessToken(string userId, string username, string email)
    {
        // Retrieve the secret key from appsettings.json
        var secretKey = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
    
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("user_id", userId),
                new Claim("username", username),
                new Claim("email", email),
            }),
            Expires = DateTime.UtcNow.AddHours(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            Issuer = issuer,
            Audience = audience
        }; 
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateJwtRefreshToken(string userId, string username, string email)
    {
        // Retrieve the secret key from appsettings.json
        var secretKey = _configuration["Jwt:Key"];
        // var secretKey =Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
    
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("user_id", userId),
                new Claim("username", username),
                new Claim("email", email),
            }),
            Expires = DateTime.UtcNow.AddDays(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string DecodeJwtAccessToken(string token)
    {
        // Your secret key for token validation (should match the key used for token generation)
        var secretKey = _configuration["Jwt:Key"];

        // Create token validation parameters
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,             // Validate the issuer
            ValidateAudience = true,           // Validate the audience
            ValidateLifetime = true,           // Validate the token expiration
            ValidateIssuerSigningKey = true,   // Validate the signing key
            ValidIssuer = _configuration["Jwt:Issuer"],     // The expected issuer
            ValidAudience = _configuration["Jwt:Audience"], // The expected audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        try
        {
            // Use JwtSecurityTokenHandler to validate and decode the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            // Extract claims from the token
            var claims = ((ClaimsIdentity)principal.Identity).Claims;

            // You can access individual claims like this:
            var userId = claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            return userId; // Return the user ID or any other information you need
        }
        catch (Exception ex)
        {
            // Token validation failed; handle the exception here
            Console.WriteLine("Token validation failed: " + ex.Message);
            return null; // Or handle the error in your specific way
        }
    }
}
