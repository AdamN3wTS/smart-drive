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
using api.IServices;
using api.Services;
using Cloudflare.NET.Core;
using Cloudflare.NET.R2;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"CWD: {Directory.GetCurrentDirectory()}");
Console.WriteLine($".env exists: {File.Exists(".env")}");
Console.WriteLine($"ENV R2__AccessKeyId: {Environment.GetEnvironmentVariable("R2__AccessKeyId") is not null}");
Console.WriteLine($"CFG R2:AccessKeyId: {builder.Configuration["R2:AccessKeyId"] is not null}");
builder.Services.AddCloudflareApiClient(builder.Configuration);
builder.Services.AddCloudflareR2Client(builder.Configuration);
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRINGTEST");

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
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IUserDbService,UserDbService>();
builder.Services.AddScoped<IRefreshTokenDbService,RefreshTokenDbService>();
builder.Services.AddScoped<IMediaUploadService,MediaUploadService>();
builder.Services.AddScoped<IFileDbService,FileDbService>();



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
public partial class Program { }
