using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Wheel.JWT;
using Wheel.Middlewares;
using Wheel.Models;
using Wheel.Models.Connections;
using Wheel.Repos;
using Wheel.Services;
using Wheel.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Connector>(builder.Configuration.GetSection("Connection"));
builder.Services.AddOptions();

builder.Services.AddScoped<IValidator<UserRegisterModel>, UserRegisterModelValidation>();
builder.Services.AddSingleton<ICustomerRepo, CustomerRepo>();
builder.Services.AddSingleton<IWheelGameService, WheelGameService>();
builder.Services.AddSingleton<IJWTGenerator, JWTGenerator>();

builder.Services.AddAuthentication("DS")//Default scheme
    .AddJwtBearer("DS"/* Schema name */, o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ajlsfhlkajhdsakljhfkljhasjklhgasjdghsklhljhl")),
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
        };
        o.Events = new JwtBearerEvents()
        {
            OnMessageReceived = async (ctx) =>
            {
                Console.WriteLine("is auth middleware");
                HttpRequest req = ctx.Request;
            },
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();