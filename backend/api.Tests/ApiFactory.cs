using Microsoft.AspNetCore.Mvc.Testing;

namespace api.Tests;

public class ApiFactory : WebApplicationFactory<Program>
{
    public ApiFactory()
    {
        DotNetEnv.Env.Load(".env.test");
    }
}
