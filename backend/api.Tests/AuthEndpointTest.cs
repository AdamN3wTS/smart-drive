using System;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace api.Tests;

public class AuthEndpointTest : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    public AuthEndpointTest(ApiFactory factory)
    {
        _client=factory.CreateClient();
    }
    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/test/private");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task PublicEndpoint_WithoutToken_Returns200()
    {
        var response = await _client.GetAsync("/api/test/public");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task MeEndpoiont_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
    [Fact]
    public async Task MeEndpoiont_WithInvalidToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "xyzxyzxyzxyz");
        var response = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
    [Fact]
    public async Task Logout_WithoutToken_Returns401()
    {
        var response = await _client.PostAsync("/api/auth/logout",null);
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);

    }
    [Fact]
    public async Task Refresh_WithoutCookie_Returns400()
    {
        var response = await _client.PostAsync("/api/auth/refresh",null);
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
    }

}
