namespace Photography.Infrastructure.Email;

/// <summary>
/// Email-sender configuration. Mirrors the <c>Storage</c> options pattern: a
/// top-level <see cref="Provider"/> string selects between the SMTP and no-op
/// implementations (any value other than <c>Smtp</c>, including missing config,
/// resolves to the no-op sender). The SMTP-specific fields are bound from the
/// <c>Email:Smtp</c> section.
/// </summary>
public sealed class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>One of <c>Smtp</c> or <c>NoOp</c>. Defaults to <c>NoOp</c>.</summary>
    public string Provider { get; set; } = "NoOp";

    /// <summary>Default From address used when an outgoing message doesn't override it.</summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>Optional human-friendly From display name.</summary>
    public string? FromDisplayName { get; set; }

    /// <summary>Recipient for site-wide notifications (e.g. contact-form submissions).</summary>
    public string AdminRecipient { get; set; } = string.Empty;
}

public sealed class SmtpOptions
{
    public const string SectionName = "Email:Smtp";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }

    /// <summary>
    /// <c>true</c> for implicit TLS (port 465), <c>false</c> for STARTTLS (587).
    /// Most modern providers prefer STARTTLS; defaults accordingly.
    /// </summary>
    public bool UseSsl { get; set; }

    /// <summary>Connection / send timeout, in seconds.</summary>
    public int TimeoutSeconds { get; set; } = 30;
}
