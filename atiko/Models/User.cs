using System;
using System.Collections.Generic;

namespace atiko.Models;

public class User
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? AvatarUri { get; set; }
    public string? Description { get; set; }
    public string? Gender { get; set; }
    public DateTimeOffset? Birthday { get; set; }
    public string? EmailVerifiedAt { get; set; }
    public string? ResetPasswordToken { get; set; }
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public string? LastLogin { get; set; }
    public int? NotificationCount { get; set; }
    public string? ImageUri { get; set; }
    public bool HasImage => !string.IsNullOrEmpty(ImageUri);
    public List<string> ItemIds { get; set; } = new List<string>();
    public List<string> UserTags { get; set; } = new List<string>();
}