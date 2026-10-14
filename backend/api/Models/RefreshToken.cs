using System;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace api.Models;

public class RefreshToken : BasicEntity
{
    public required string Hash {get;set;}
    public DateTime ExpAt {get;set;}
    public DateTime? RevokedAt {get;set;}
    public string? RevokedReason {get;set;}
    public User User {get;set;}
    public Guid UserId {get;set;}
    public RefreshToken? Parent {get;set;}
    public Guid? ParentId {get;set;}
}
