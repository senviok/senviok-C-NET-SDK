namespace Senviok.Models.Templates;

/// <summary>
/// A reusable email or notification template.
/// </summary>
public class Template
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? HtmlContent { get; set; }
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
}

/// <summary>
/// Parameters for creating a new template.
/// </summary>
public class CreateTemplateRequest
{
    /// <summary>Human-readable identifier or name for the template.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Default subject line for emails generated from this template.</summary>
    public string? Subject { get; set; }

    /// <summary>HTML template content supporting dynamic placeholder tags.</summary>
    public string? HtmlContent { get; set; }
}

/// <summary>
/// Parameters for updating an existing template.
/// </summary>
public class UpdateTemplateRequest
{
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public string? HtmlContent { get; set; }
}
