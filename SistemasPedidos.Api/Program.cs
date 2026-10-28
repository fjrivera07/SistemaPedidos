using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SistemaPedidos.Api.Services;
using SistemaPedidos.Application.Commands.CancelarPedido;
using SistemaPedidos.Application.Commands.CrearPedido;
using SistemaPedidos.Application.Interfaces;
using SistemaPedidos.Application.Queries.ObtenerPedido;
using SistemaPedidos.Infrastructure.Data;
using SistemaPedidos.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

var tenantId = builder.Configuration["AzureAd:TenantId"];

var apiScope =
    "api://d33f9f16-f4b0-4575-9299-75be4c54bdbf/Pedidos.Read";

builder.Services.AddSwaggerGen(options =>
{
    // JWT local
    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

    // Microsoft Entra ID
    options.AddSecurityDefinition("EntraID",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(
                        $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),

                    TokenUrl = new Uri(
                        $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),

                    Scopes = new Dictionary<string, string>
                    {
                        [apiScope] = "Consultar pedidos"
                    }
                }
            }
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("EntraID", document)] =
                [apiScope]
        });
});

builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<CrearPedidoHandler>();
builder.Services.AddScoped<ObtenerPedidoHandler>();
builder.Services.AddScoped<CancelarPedidoHandler>();
builder.Services.AddScoped<JwtTokenService>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "No se configuró la clave JWT.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException(
        "La clave JWT debe tener al menos 32 bytes.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],

                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                RoleClaimType = "role",

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services
    .AddAuthentication()
    .AddMicrosoftIdentityWebApi(
        builder.Configuration.GetSection("AzureAd"),
        jwtBearerScheme: "EntraID");

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId(
            "83d76911-63b0-4d02-b202-ba87946d6f4f");

        options.OAuthAppName("SistemaPedidos.Client");

        options.OAuthUsePkce();

        options.OAuthScopes(
            "api://d33f9f16-f4b0-4575-9299-75be4c54bdbf/Pedidos.Read");

        options.OAuth2RedirectUrl(
            "https://localhost:7064/swagger/oauth2-redirect.html");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
