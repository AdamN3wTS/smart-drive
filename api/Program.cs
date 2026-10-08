using System.Text;
using api.Helpers;
using Microsoft.IdentityModel.Tokens;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
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
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
