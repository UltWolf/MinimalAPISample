using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalAPISample.Context;
using MinimalAPISample.Entity;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalAPISample.Modules
{
    public static class UserModule
    {
        public static WebApplication AddUserEndpointsModule(this WebApplication app)
        {
            app.MapPost("/register", async (string username, string password, ApplicationDbContext dbContext) =>
            {
                if (await dbContext.Users.AnyAsync(u => u.Username == username))
                {
                    return Results.BadRequest("User already exists.");
                }
                var user = new User { Username = username, Password = password };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
                return Results.Ok("User registered successfully.");
            });

            app.MapPost("/login", async (string username, string password, ApplicationDbContext dbContext) =>
            {
                var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Username == username && u.Password == password);
                if (user == null)
                {
                    return Results.Unauthorized();
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
            new Claim(ClaimTypes.Name, username)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("SuperSecretKey")), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Results.Ok(new
                {
                    Token = tokenString
                });
            });

            app.MapGet("/protected", [Authorize] () =>
            {
                return Results.Ok("This is a protected endpoint.");
            }).WithMetadata(new SwaggerOperationAttribute(summary: "Protected endpoint", description: "Requires authentication")); ;
            return app;
        }
    }
}
