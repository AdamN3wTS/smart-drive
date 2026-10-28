using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using api.Dtos;
using Cloudflare.NET.Security.Rulesets.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.StaticFiles;

namespace api.Tests;

[Trait("Category", "File")]

public class FileEndpointTest: IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly HttpClient _clientNoCookie;

    public FileEndpointTest(ApiFactory factory)
    {
        _client=factory.CreateClient();
        _clientNoCookie =factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = false
        });
    }
    [Fact]
    public async Task ProtectedFileUploadEndpoint_WithoutToken_Returns401()
    {
        var response = await _client.PostAsync("/api/files/upload",null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task FileUploadEndpoint_ValidDto_ReturnsUrl()
    {
        var body = new {username=$"Adam_{Guid.NewGuid().ToString("N")[..8]}",Email=$"{Guid.NewGuid()}@gmail.pl",Password="Password123!",VerifyPassword="Password123!"};
        var responseAuth = await _client.PostAsJsonAsync("/api/auth/register",body);
        
        Assert.Equal(HttpStatusCode.OK, responseAuth.StatusCode); 

        var responseBodyAuth = await responseAuth.Content.ReadFromJsonAsync<ResponseDto<RegisterResponseDto>>();
        Assert.NotNull(responseBodyAuth);
        Assert.NotNull(responseBodyAuth.Data);
        Assert.False(string.IsNullOrWhiteSpace(responseBodyAuth.Data.accessToken));
        _client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", responseBodyAuth.Data.accessToken);
        var path = Path.Combine(AppContext.BaseDirectory, "files", "image.png");
        var fileInfo = new FileInfo(path);

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileInfo.Name, out var contentType))
            contentType = "application/octet-stream";

        var uploadDto = new UploadFileDto
        {
            Name = fileInfo.Name,
            ContentLength = fileInfo.Length,
            ContentType = contentType,
        };
        var response = await _client.PostAsJsonAsync<UploadFileDto>("/api/files/upload",uploadDto);

        var bodyy = await response.Content.ReadFromJsonAsync<ResponseDto<string?>>();
        Assert.False(String.IsNullOrWhiteSpace(bodyy.Data));
        
    }
}
