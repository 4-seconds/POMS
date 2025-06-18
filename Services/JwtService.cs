using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PurchaseOrderManagementSystem.Models; // Assuming ApplicationUser is in this namespace
using Microsoft.AspNetCore.Identity;

namespace PurchaseOrderManagementSystem.Services
{
    /// <summary>
    /// Service for generating JSON Web Tokens (JWTs) for authenticated users.
    /// This service encapsulates the logic for creating secure tokens that can be used
    /// for authorization in API requests.
    /// </summary>
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtService"/> class.
        /// </summary>
        /// <param name="configuration">The application's configuration, used to retrieve JWT settings.</param>
        /// <param name="userManager">The user manager for managing user roles.</param>
        public JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        /// <summary>
        /// Generates a JWT token for the specified application user.
        /// </summary>
        /// <param name="user">The <see cref="ApplicationUser"/> for whom the token is to be generated.</param>
        /// <returns>A string representing the generated JWT token.</returns>
        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            // Retrieve JWT settings (Secret, Issuer, Audience) from the application's configuration.
            // These settings are typically defined in appsettings.json or environment variables.
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"] ?? "thisisalongandverysecuresecretkeyforjwtauthentication";
            var issuer = jwtSettings["Issuer"] ?? "your-app";
            var audience = jwtSettings["Audience"] ?? "your-app-users";

            // Create a SymmetricSecurityKey from the secret. This key is used to sign the token.
            // Symmetric keys are used for both signing and verifying the token.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            // Create SigningCredentials using the security key and HmacSha256 algorithm.
            // These credentials are used to digitally sign the JWT, ensuring its integrity.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Create claims list
            var claims = new List<Claim>
            {
                // JWT ID claim: Provides a unique identifier for the JWT.
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Name Identifier claim: A unique identifier for the user, usually the user's ID from IdentityUser.
                new Claim(ClaimTypes.NameIdentifier, user.Id), 
                // Name claim: The user's full name for general display purposes.
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                // Email claim: The user's email address.
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create the JWT security token.
            var token = new JwtSecurityToken(
                issuer: issuer, // The issuer of the token
                audience: audience, // The audience for whom the token is intended
                claims: claims, // The claims to be included in the token
                expires: DateTime.Now.AddHours(24), // The token's expiration time (24 hours from creation)
                signingCredentials: credentials); // The credentials used to sign the token

            // Serialize the JWT token to a compact string format.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

