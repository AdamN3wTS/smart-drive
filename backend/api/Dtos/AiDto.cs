using System;

namespace api.Dtos;

public class FastApiResponseWrapper<T>
{
    public string Status { get; set; } = string.Empty;
    public T? Data { get; set; }
}
public class AiSearchResponseDto
{
    public double Score {get;set;}
    public int? Page {get;set;}
    public string? Text {get;set;}
    public string? Source {get;set;}

}
