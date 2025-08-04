using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InvoiceInsightAPI.Handlers;
using InvoiceInsightAPI.Handlers.Interfaces;
using InvoiceInsightAPI.Models.Request;
using InvoiceInsightAPI.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProcessInvoiceHandler, ProcessInvoiceHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("ProcessInvoice", [Authorize(Roles = "Admin")] async (
        IProcessInvoiceHandler processInvoiceHandler, 
        [FromBody] ProcessInvoiceRequest req) =>
{ 
    var result = await processInvoiceHandler.ProcessInvoiceAsync(req);
    return result;
})
.Produces<ApiResponse<ProcessInvoiceResponse>>(200)
.Produces<ApiResponse<ProcessInvoiceResponse>>(400)
.Produces<ApiResponse<ProcessInvoiceResponse>>(500);

app.MapPost("/GenerateToken", (IConfiguration configuration) =>
{
    var claims = new[]
    {
        new Claim("role", "Admin")
    };
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials
    );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { token = tokenString });
});

app.UseHttpsRedirection();
app.Run();