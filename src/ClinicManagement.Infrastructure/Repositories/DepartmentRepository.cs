using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IDepartmentRepository.
/// </summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(ClinicDbContext context, ILogger<DepartmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptNo == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching department by ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.DeptName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all departments");
            return Enumerable.Empty<Department>();
        }
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptName == name, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching department by name: {Name}", name);
            return null;
        }
    }

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding department: {Name}", department.DeptName);
            throw;
        }
    }

    public async Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department ID: {Id}", department.DeptNo);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments.AnyAsync(d => d.DeptNo == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking department existence for ID: {Id}", id);
            return false;
        }
    }
}
