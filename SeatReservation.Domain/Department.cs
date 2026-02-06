// SeatReservation.Domain

namespace SeatReservation.Domain;

/// <summary>
/// For tree list hierarchy of departments.
/// </summary>
public class Department
{
    private readonly List<Department> _childDepartments = [];
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    // EF Core
    private Department()
    {
    }

    public DepartmentId Id { get; private set; }

    public DepartmentName Name { get; private set; } = null!;

    public Identifier Identifier { get; private set; } = null!;

    public DepartmentId? ParentId { get; private set; }

    public Department? Parent { get; private set; }

    public Path Path { get; private set; }

    public int Depth { get; private set; }

    public IEnumerable<DepartmentLocation> Locations => _locations;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<Department> ChildDepartments => _childDepartments;

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _locations;

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _positions;

    private Department(DepartmentId id, DepartmentId? parentId, DepartmentName name, Identifier identifier, Path path, int depth, IEnumerable<DepartmentLocation> locations)
    {
        Id = id;
        Name = name;
        Identifier = identifier;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _locations = locations.ToList();
    }

    public static Department CreateParent(DepartmentName name, Identifier identifier, IEnumerable<DepartmentLocation> locations, DepartmentId? departmentId = null)
    {
        var departmentLocations = locations.ToList();

        if (departmentLocations.Count == 0)
        {
            throw new ArgumentException("A department must have at least one location.", nameof(locations));
        }

        var path = Path.CreateParent(identifier);

        return new Department(
            departmentId ?? new DepartmentId(Guid.NewGuid()),
            null,
            name: name,
            identifier: identifier,
            path: path,
            depth: 0,
            locations: departmentLocations);
    }

    public static Department CreateChild(DepartmentName name, Identifier identifier, Department parent, IEnumerable<DepartmentLocation> locations, DepartmentId? departmentId = null)
    {
        var departmentLocations = locations.ToList();

        if (departmentLocations.Count == 0)
        {
            throw new ArgumentException("A department must have at least one location.", nameof(locations));
        }

        var path = Path.CreateParent(identifier);

        return new Department(
            departmentId ?? new DepartmentId(Guid.NewGuid()),
            parentId: parent.Id,
            name: name,
            identifier: identifier,
            path: path,
            depth: parent.Depth + 1,
            locations: departmentLocations);
    }
}

public record DepartmentId(Guid Value);

public record DepartmentLocation(string Value);

public record DepartmentPosition(string Value);

public record DepartmentName()
{
    public const int NAME_MAX_LENGTH = 100;

    public string Value
    {
        get;
        private set;
    }

    private DepartmentName(string value)
        : this()
    {
        Value = value;
    }

    public static DepartmentName Create(string value)
    {
        return value == null
            ? throw new ArgumentNullException(paramName: nameof(value))
            : new DepartmentName(value);
    }
};

public record Identifier()
{
    public const int IDENTIFIER_MAX_LENGTH = 100;

    public string Value
    {
        get;
        private set;
    }

    private Identifier(string value)
        : this()
    {
        Value = value;
    }

    public static Identifier Create(string value)
    {
        return new Identifier(value);
    }
};

public record Path
{
    // public const int PATH_MAX_LENGTH = 100;
    private const char Separator = '.';

    public string Value
    {
        get;
        private set;
    }

    private Path(string value)
    {
        Value = value;
    }

    private Path(Guid identifierValue)
    {
    }

    public static Path Create(string value)
    {
        return new Path(value);
    }

    public static Path CreateParent(Identifier identifier)
    {
        return new Path(identifier.Value);
    }

    public Path CreateChild(Identifier childIdentifier)
    {
        return new Path(Value + Separator + childIdentifier.Value);
    }
}