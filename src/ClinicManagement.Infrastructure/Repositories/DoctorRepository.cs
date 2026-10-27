using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IDoctorRepository.
/// </summary>
public class DoctorRepository : IDoctorRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DoctorRepository> _logger;

    public DoctorRepository(ClinicDbContext context, ILogger<DoctorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctor by ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Doctor>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .Where(d => d.Status == 1)
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all active doctors");
            return Enumerable.Empty<Doctor>();
        }
    }

    public async Task<IEnumerable<Doctor>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .Where(d => d.Status == 1 && d.Name.Contains(searchQuery))
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors with query: {Query}", searchQuery);
            return Enumerable.Empty<Doctor>();
        }
    }

    public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Department)
                .Where(d => d.Status == 1 && d.Department != null && d.Department.DeptName == deptName)
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctors for department: {DeptName}", deptName);
            return Enumerable.Empty<Doctor>();
        }
    }

    public async Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor: {Name}", doctor.Name);
            throw;
        }
    }

    public async Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor ID: {Id}", doctor.DoctorId);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoginTable
                .AnyAsync(l => l.Email == email && l.Type == 2, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking doctor email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctor = await _context.Doctors.FindAsync(new object[] { id }, cancellationToken);
            if (doctor == null) return false;

            doctor.Status = 0; // Mark as left
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft-deleting doctor ID: {Id}", id);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Doctors.AnyAsync(d => d.DoctorId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking doctor existence for ID: {Id}", id);
            return false;
        }
    }
}
