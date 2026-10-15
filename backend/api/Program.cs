using System.Text;
using api.Data;
using api.Helpers;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using api.IDbServices;
using api.DbServices;
using api.IDbService;
using api.DbService;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

if (connectionString == null)
{
    throw new NullReferenceException("ConnectionString==null");
}
Console.WriteLine("1");
builder.Services.AddDbContext<AppDbContext>(o=>o.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
builder.Services.AddControllers();
builder.Services.AddOpenApi();
Console.WriteLine("2");

builder.Services.AddAuthentication().AddJwtBearer(o =>
{
    var key = SafeEnvironment.GetSafeEnvironment("JWT_KEY");
    var audience = SafeEnvironment.GetSafeEnvironment("JWT_AUDIENCE");
    var issuer = SafeEnvironment.GetSafeEnvironment("JWT_ISSUER");


    o.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidAudience=audience,
        ValidIssuer=issuer,
        ValidateLifetime = true,
        ValidateIssuerSigningKey=true,
        ValidateIssuer=true,
        ValidateAudience=true,
        ClockSkew=TimeSpan.Zero

    };
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthService,AuthService>();
builder.Services.AddSingleton<IUserDbService,UserDbService>();
builder.Services.AddSingleton<IRefreshTokenDbService,RefreshTokenDbService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
