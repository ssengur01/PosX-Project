using Employees.Domain.Enums;

namespace Employees.Domain.Entities;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    private readonly List<PermissionType> _permissions = new();
    public IReadOnlyCollection<PermissionType> Permissions => _permissions.AsReadOnly();

    private Role()
    {
        Name = string.Empty;
    }

    public Role(string name, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
    }

    public void AddPermission(PermissionType permission)
    {
        if (!_permissions.Contains(permission))
        {
            _permissions.Add(permission);
        }
    }

    public void RemovePermission(PermissionType permission)
    {
        _permissions.Remove(permission);
    }

    public bool HasPermission(PermissionType permission)
    {
        return _permissions.Contains(permission);
    }
}
