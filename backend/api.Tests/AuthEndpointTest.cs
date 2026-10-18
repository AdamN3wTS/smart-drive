using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using api.Dtos;
using api.Helpers;
using api.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;

namespace api.Tests;

public class AuthEndpointTest : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly HttpClient _clientNoCookie;

    public AuthEndpointTest(ApiFactory factory)
    {
        _client=factory.CreateClient();
        _clientNoCookie =factory.CreateClient(new WebApplicationFactoryClientOptions
{
            HandleCookies = false
        });
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
    [Fact]
    public async Task Register_EmptyBody_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register",new {});
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
    }
    [Fact]
    public async Task Login_EmptyBody_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",new {});
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
    }
    [Fact]
    public async Task Register_ValidData_Returns200()
    {
        var body = new {username="Adam",Email=$"{Guid.NewGuid()}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var response = await _client.PostAsJsonAsync("/api/auth/register",body);
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
    }
    [Fact]
    public async Task Register_ValidData_ReturnsAccessToken()
    {
        var body = new {username=$"Adam_{Guid.NewGuid().ToString("N")[..8]}",Email=$"{Guid.NewGuid()}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var response = await _client.PostAsJsonAsync("/api/auth/register",body);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 

        var responseBody = await response.Content.ReadFromJsonAsync<ResponseDto<RegisterResponseDto>>();
        Assert.NotNull(responseBody);
        Assert.NotNull(responseBody.Data);
        Assert.False(string.IsNullOrWhiteSpace(responseBody.Data.accessToken));

    }
    [Fact]
    public async Task Register_ValidData_SetsRefreshTokenCookie()
    {
        var body = new {username=$"Adam_{Guid.NewGuid().ToString("N")[..8]}",Email=$"{Guid.NewGuid()}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var response = await _client.PostAsJsonAsync("/api/auth/register",body);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 

        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookies),
        "not Set-Cookie");
        var refreshToken = cookies.FirstOrDefault(p=>p.StartsWith("refreshToken"));
        Assert.NotNull(refreshToken);

    }
    [Fact]
    public async Task Register_ExistingEmail_Returns400()
    {
        var body = new {username=$"Adam_{Guid.NewGuid().ToString("N")[..8]}",Email=$"{Guid.NewGuid()}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var response = await _client.PostAsJsonAsync("/api/auth/register",body);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        var response2 = await _client.PostAsJsonAsync("/api/auth/register",body);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode); 
    }
    [Fact]
    public async Task Login_CorrectPassword_Returns200()
    {
        var username=$"Adam_{Guid.NewGuid().ToString("N")[..8]}";
        var body = new {Username=username,Email=$"{username}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var response = await _client.PostAsJsonAsync("/api/auth/register",body);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        var bodyLogin = new {NameEmail=username,Password="Password123!"};
        var response2 = await _client.PostAsJsonAsync("/api/auth/login",bodyLogin);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode); 
    }

    [Fact]
    public async Task Login_WrongPassword_Returns400()
    {
        var username=$"Adam_{Guid.NewGuid().ToString("N")[..8]}";
        var body = new {Username=username,Email=$"{username}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var response = await _client.PostAsJsonAsync("/api/auth/register",body);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        var bodyLogin = new {NameEmail=username,Password="Password123!!"};
        var response2 = await _client.PostAsJsonAsync("/api/auth/login",bodyLogin);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode); 
    }
    [Fact]
    public async Task Login_NonExistingUser_Returns400()
    {
        var bodyLogin = new {NameEmail="username:)",Password="Password123!!"};
        var response2 = await _client.PostAsJsonAsync("/api/auth/login",bodyLogin);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode); 
    }
    [Fact]
    public async Task Me_WithValidToken_Returns200()
    {
        var username=$"Adam_{Guid.NewGuid().ToString("N")[..8]}";
        var body = new {Username=username,Email=$"{username}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var response = await _client.PostAsJsonAsync("/api/auth/register",body);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 

        var responseBody = await response.Content.ReadFromJsonAsync<ResponseDto<RegisterResponseDto>>();
        Assert.NotNull(responseBody);
        Assert.NotNull(responseBody.Data);
        Assert.False(string.IsNullOrWhiteSpace(responseBody.Data.accessToken));
        _client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", responseBody.Data.accessToken);
        var responseMe = await _client.GetAsync("/api/auth/me");
        
        Assert.Equal(HttpStatusCode.OK,responseMe.StatusCode);
    }
    [Fact]
    public async Task Refresh_WithValidCookie_ReturnsNewAccessToken()
    {
        var username=$"Adam_{Guid.NewGuid().ToString("N")[..8]}";
        var body = new {Username=username,Email=$"{username}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var response = await _client.PostAsJsonAsync("/api/auth/register",body);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 

        var responseBody = await response.Content.ReadFromJsonAsync<ResponseDto<RegisterResponseDto>>();
        Assert.NotNull(responseBody);
        Assert.NotNull(responseBody.Data);
        Assert.False(string.IsNullOrWhiteSpace(responseBody.Data.accessToken));
        _client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", responseBody.Data.accessToken);
        var responseRefresh = await _client.PostAsync("/api/auth/refresh",null);
        var responseRefreshBody =await  responseRefresh.Content.ReadFromJsonAsync<ResponseDto<LoginResponseDto>>();
        Assert.NotNull(responseRefreshBody);
        Assert.NotNull(responseRefreshBody.Data);
        Assert.False(string.IsNullOrWhiteSpace(responseRefreshBody.Data.accessToken));
    }
    [Fact]
    public async Task Refresh_UsedTwice_SecondReturns401()
    {
        
        var username = $"Adam_{Guid.NewGuid().ToString("N")[..8]}";
        var body = new
        {
            Username = username,
            Email = $"{username}@gmail.pl",
            Password = "Password123!",
            VerifyPassword = "Password123!"
        };

        var registerResponse = await _clientNoCookie.PostAsJsonAsync("/api/auth/register", body);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var refreshToken = GetCookieValue(registerResponse, "refreshToken");
        Assert.False(string.IsNullOrEmpty(refreshToken));

        var first = await SendRefresh(refreshToken);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var firstBody = await first.Content.ReadFromJsonAsync<ResponseDto<LoginResponseDto>>();
        Assert.NotNull(firstBody?.Data);
        Assert.False(string.IsNullOrWhiteSpace(firstBody.Data.accessToken));

        var second = await SendRefresh(refreshToken);

        Assert.Equal(HttpStatusCode.Unauthorized, second.StatusCode);
    }

    private Task<HttpResponseMessage> SendRefresh(string refreshToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh");
        request.Headers.Add("Cookie", $"refreshToken={refreshToken}");
        return _clientNoCookie.SendAsync(request);
    }

    private static string? GetCookieValue(HttpResponseMessage response, string name)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
            return null;

        return SetCookieHeaderValue.ParseList(cookies.ToList())
            .FirstOrDefault(c => c.Name == name)
            ?.Value.ToString();
    }
    [Fact]
    public async Task Logout_ThenRefresh_Returns401()
    {
        var username=$"Adam_{Guid.NewGuid().ToString("N")[..8]}";
        var body = new {Username=username,Email=$"{username}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var response = await _client.PostAsJsonAsync("/api/auth/register",body);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 

        var responseBody = await response.Content.ReadFromJsonAsync<ResponseDto<RegisterResponseDto>>();
        Assert.NotNull(responseBody);
        Assert.NotNull(responseBody.Data);
        Assert.False(string.IsNullOrWhiteSpace(responseBody.Data.accessToken));
        _client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", responseBody.Data.accessToken);
        var responseLogout = await _client.PostAsync("/api/auth/logout",null);
        Assert.Equal(HttpStatusCode.OK,responseLogout.StatusCode);
        var responseRefresh = await _client.PostAsync("/api/auth/refresh",null);
        Assert.Equal(HttpStatusCode.Unauthorized,responseRefresh.StatusCode);
    }
}
