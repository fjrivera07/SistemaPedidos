using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Introduce el token JWT."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
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

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
