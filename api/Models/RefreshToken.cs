using System;

namespace api.Models;

public class RefreshToken : BasicEntity
{
    public string Hash {get;set;}
    public DateTime ExpAt {get;set;}
    public DateTime? RevokedAt {get;set;}
    public string? RevokedReason {get;set;}
    public RefreshToken? Parent {get;set;}
    public Guid ParentId {get;set;}
}
