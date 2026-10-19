using TMSystem.Domain.Common;
using TMSystem.Domain.Exceptions;

namespace TMSystem.Domain.Entities;

public sealed class Label : AuditableEntity
{
    public const int NameMaxLength = 50;

    public string Name { get; private set; } = string.Empty;
    public string Color { get; private set; } = "#808080";
    public Guid ProjectId { get; private set; }

    private Label() { } // EF Core üçün

    public static Label Create(string name, string color, Guid projectId)
    {
        if (projectId == Guid.Empty)
            throw new DomainException("Label bir layihəyə aid olmalıdır.");

        var label = new Label { ProjectId = projectId };
        label.Update(name, color);
        return label;
    }

    public void Update(string name, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Label adı boş ola bilməz.");
        if (name.Length > NameMaxLength)
            throw new DomainException($"Label adı {NameMaxLength} simvoldan uzun ola bilməz.");
        if (!IsHexColor(color))
            throw new DomainException("Rəng #RRGGBB formatında olmalıdır.");

        Name = name.Trim();
        Color = color.ToUpperInvariant();
    }

    private static bool IsHexColor(string color) =>
        color is { Length: 7 } && color[0] == '#' && color[1..].All(Uri.IsHexDigit);
}
