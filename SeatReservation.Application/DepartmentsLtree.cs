// SeatReservation.Application

using Dapper;
using SeatReservation.Application.DataBase;

namespace SeatReservation.Application;

public record DepartmentDto
{
    public Guid Id { get; }

    public Guid? ParentId { get; }

    public string Name { get; } = null!;

    public string Identifier { get; } = null!;

    public string Path { get; } = null!;

    public int Depth { get; }

    public bool IsActive { get; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; }

    public List<DepartmentDto> Children { get; set; } = [];

    public bool HasMoreChildren { get; set; }
}

public class DepartmentsLTree
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DepartmentsLTree(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<DepartmentDto>> GetHierarchyRecursive(string rootPath, CancellationToken ct)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(ct);

        DepartmentDto? getEventDto = null;

        // ltree запрос на потомков
        const string ltreeSql = """
                                SELECT 
                                    id,
                                    parent_id,
                                    name,
                                    identifier,
                                    path,
                                    depth,
                                    is_active,
                                    created_at,
                                    updated_at
                                FROM departments d 
                                WHERE subpath(d.path, 0, nlevel(d.path) - 1) = subpath(@rootPath::ltree, 0, nlevel(@rootPath::ltree) - 1)
                                    AND d.path != @rootPath::ltree
                                ORDER BY depth
                                """;

        // ltree запрос на получение соседей
        const string neighborsSql = """
                                    SELECT 
                                        id,
                                        parent_id,
                                        name,
                                        identifier,
                                        path,
                                        depth,
                                        is_active,
                                        created_at,
                                        updated_at
                                    FROM departments
                                    WHERE path ~ @rootPath::ltree
                                    ORDER BY depth
                                    """;

        // если глубина будет большая, вариант с рекурсией будет плохим решением
        const string dapperSql = """
                                 WITH RECURSIVE dept_tree AS (
                                     SELECT d.*, 0 AS level
                                     FROM departments d
                                     WHERE d.path = @rootPath::ltree
                                     UNION ALL
                                     SELECT c.*, dt.level + 1 AS level
                                     FROM departments c
                                     JOIN dept_tree dt ON c.parent_id = dt.id)
                                     SELECT 
                                         id,
                                         parent_id,
                                         name,
                                         identifier,
                                         path,
                                         depth,
                                         is_active,
                                         created_at,
                                         updated_at,
                                         level
                                     FROM dept_tree
                                     ORDER BY level, id
                                 )
                                 """;

        var departmentRaws = (await connection.QueryAsync<DepartmentDto>(dapperSql, new {rootPath})).ToList();

        var departmentsDict = departmentRaws.ToDictionary(d => d.Id);
        var roots = new List<DepartmentDto>();

        foreach (DepartmentDto row in departmentRaws)
        {
            if (row.ParentId.HasValue && departmentsDict.TryGetValue(row.ParentId.Value, out var parent))
            {
                parent.Children.Add(departmentsDict[row.Id]);
            }
            else
            {
                roots.Add(departmentsDict[row.Id]);
            }
        }

        return roots;
    }

    public async Task<List<DepartmentDto>> GetHierarchy(string rootPath, int depth, CancellationToken ct)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(ct);

        DepartmentDto? getEventDto = null;

        // ltree поиск с гулбиной
        const string ltreeDepthSql = """
                                     SELECT 
                                         id,
                                         parent_id,
                                         name,
                                         identifier,
                                         path,
                                         depth,
                                         is_active,
                                         created_at,
                                         updated_at
                                         FROM departments
                                         WHERE path <@ @rootPath::ltree
                                            AND nlevel(path) > nlevel(@rootPath::ltree)
                                            AND nlevel(path) <= nlevel(@rootPath::ltree) + @depth
                                     """;

        var departmentRaws = (await connection.QueryAsync<DepartmentDto>(ltreeDepthSql, new {rootPath})).ToList();
        var departmentsDict = departmentRaws.ToDictionary(d => d.Id);
        var roots = new List<DepartmentDto>();

        foreach (DepartmentDto row in departmentRaws)
        {
            if (row.ParentId.HasValue && departmentsDict.TryGetValue(row.ParentId.Value, out var parent))
            {
                parent.Children.Add(departmentsDict[row.Id]);
            }
            else
            {
                roots.Add(departmentsDict[row.Id]);
            }
        }

        return roots;
    }

    public async Task<int> DeleteSubtreeQuery(string rootPath)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();

        // ltree дочерних элементов у узла
        const string ltreeDeleteSql = """
                                      DELETE FROM departments
                                      WHERE path <@ @rootPath::ltree AND path != @rootPath::ltree
                                      """;
        var affected = await connection.ExecuteAsync(ltreeDeleteSql, new {rootPath});

        return affected;
    }

    // Получить корневые подразделения и несколько дочерних первого уровня из каждого корневого подразделения
    // чтобы постепенно раскрывать дерево на клиенте.
    public async Task<int> GetSubtreeQuery(int rootLimit, int offset, int childLimit)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();

        // корневые подразделения с пагинацией
        const string getRootsSql = """
                                   -- родительские подразделения
                                   WITH roots AS (
                                      SELECT
                                      id,
                                      parent_id,
                                      name,
                                      identifier,
                                      path,
                                      depth,
                                      is_active,
                                      created_at,
                                      updated_at
                                      FROM departments d
                                      WHERE d.parent_id IS NULL
                                      ORDER BY created_at
                                      OFFSET @offset LIMIT @root_limit
                                      )
                                      -- получаем родительские подразделения
                                      SELECT *, (EXIST(
                                                     SELECT 1 
                                                     FROM deparments
                                                     WHERE parent_id = roots.id 
                                                     OFFSET @child_limit
                                                     LIMIT 1)) AS has_more_children true
                                      FROM roots
                                      
                                      UNION ALL
                                      
                                      -- дочерние
                                      SELECT c.*, (EXIST(
                                                            SELECT 1 
                                                            FROM deparments
                                                            WHERE parent_id = c.id 
                                                            )) AS has_more_children true
                                      FROM roots r
                                      CROSS JOIN LATERAL (
                                      SELECT
                                        id,
                                        parent_id,
                                        name,
                                        identifier,
                                        path,
                                        depth,
                                        is_active,
                                        created_at,
                                        updated_at
                                     FROM departments d
                                     WHERE d.parent_id = r.id AND d.is_active = true
                                     ORDER BY d.created_at
                                     LIMIT @child_limit
                                      ) AS c
                                   """;

        // через оконную функцию
        const string getRootsWindowSql = """
                                         WITH roots AS (
                                         SELECT
                                         d.id,
                                         d.parent_id,
                                         d.name,
                                         d.identifier,
                                         d.path,
                                         d.depth,
                                         d.is_active,
                                         d.created_at,
                                         d.updated_at
                                         FROM departments d
                                         WHERE d.parent_id IS NULL
                                         ORDER BY d.created_at
                                         OFFSET @offset LIMIT @root_limit
                                         ),
                                         
                                         ranked_children AS ( 
                                         SELECT 
                                         d.id,
                                         d.parent_id,
                                         d.name,
                                         d.identifier,
                                         d.path,
                                         d.depth,
                                         d.is_active,
                                         d.created_at,
                                         d.updated_at
                                         ROW_NUMBER() OVER (PARTITION BY parent_id ORDER BY created_at) AS child_rank
                                         FROM departments d
                                         JOIN roots r ON d.parent_id = r.id
                                         WHERE d.is_active = true
                                         )
                                         
                                         SELECT
                                         r.id
                                         r.parent_id,
                                         r.name,
                                         r.identifier,
                                         r.path,
                                         r.depth,
                                         r.is_active,
                                         r.created_at,
                                         r.updated_at,
                                         (EXIST(
                                         SELECT 1 
                                         FROM deparments
                                         WHERE parent_id = roots.id 
                                         OFFSET @child_limit
                                         LIMIT 1)) AS has_more_children true
                                         FROM roots r
                                         
                                         UNION ALL
                                         
                                         SELECT
                                         rc.id
                                         rc.parent_id,
                                         rc.name,
                                         rc.identifier,
                                         rc.path,
                                         rc.depth,
                                         rc.is_active,
                                         rc.created_at,
                                         rc.updated_at,
                                         (EXIST(
                                         SELECT 1 
                                         FROM deparments
                                         WHERE parent_id = c.id 
                                         )) AS has_more_children true
                                         FROM ranked_children rc
                                         WHERE rc.child_rank <= @child_limit
                                         """;
        var affected = await connection.ExecuteAsync(getRootsSql, new { rootLimit, offset, childLimit });

        return affected;
    }
}