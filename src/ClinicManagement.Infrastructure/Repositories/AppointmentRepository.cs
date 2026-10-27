using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IAppointmentRepository.
/// </summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<AppointmentRepository> _logger;

    public AppointmentRepository(ClinicDbContext context, ILogger<AppointmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching appointment by ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching appointments for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentStatus == 2) // Pending
                .OrderBy(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pending appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId
                    && a.AppointmentStatus == 1 // Approved
                    && a.Date.HasValue
                    && a.Date.Value.Date == today)
                .OrderBy(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching today's appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId
                    && a.AppointmentStatus == 1 // Approved
                    && a.Date.HasValue
                    && a.Date.Value.Date == today)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current appointment for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetBillHistoryByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.AppointmentStatus == 3) // Completed
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bill history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetTreatmentHistoryByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.AppointmentStatus == 3) // Completed
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching treatment history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment?> GetPendingFeedbackByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId
                    && a.AppointmentStatus == 3 // Completed
                    && a.FeedbackStatus == 2) // Pending
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pending feedback for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<Appointment?> GetNotificationByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId
                    && a.PatientNotification == 2) // Unseen
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching notification for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetBillableByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId
                    && a.AppointmentStatus == 1 // Approved
                    && a.Disease != null) // Has prescription
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching billable appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all appointments");
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding appointment");
            throw;
        }
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment ID: {Id}", appointment.AppointId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AnyAsync(a => a.AppointId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking appointment existence for ID: {Id}", id);
            return false;
        }
    }

    public async Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAndPatientAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Return future approved appointments for the doctor that have no patient assigned
            return await _context.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorId == doctorId
                    && a.PatientId == null
                    && a.Date.HasValue
                    && a.Date.Value > DateTime.Now)
                .OrderBy(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching free slots for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }
}
