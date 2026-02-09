using BuildingBlocks.Common.BaseClasses;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public class Role : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public RoleType RoleType { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private Role() { } // EF Core

    public Role(string name, string description, RoleType roleType)
    {
        Name = name;
        Description = description;
        RoleType = roleType;
    }

    public void UpdateRole(string name, string description)
    {
        Name = name;
        Description = description;
        ModifiedAt = DateTime.UtcNow;
    }
}
