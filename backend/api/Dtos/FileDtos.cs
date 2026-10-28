using System;

namespace api.Dtos;

public class UploadFileDto
{
    public string Name {get;set;}
    public string ContentType {get;set;}
    public long ContentLength {get;set;}
}
public class UploadFileResponseDto
{
    public string Url {get;set;}
    public Guid FileId {get;set;}

}
