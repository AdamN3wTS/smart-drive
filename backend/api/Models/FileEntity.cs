using System;
using api.Models.Enums;

namespace api.Models;

public class FileEntity : BasicEntity
{
    public string? FileName {get;set;}
    public string? FileUrl {get;set;}
    public FileStatus Status {get;set;}
    public string? ContentType {get;set;}
    public long? ContentLength {get;set;}
    public string Key {get;set;}
    public User User {get;set;}
    public Guid UserId {get;set;}

}
