using MedicalBackend.Database;
using MedicalBackend.DTOs;
using MedicalBackend.Entities;
using MedicalBackend.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MedicalBackend.Repositories;

public class AppointmentRepository: BaseRepository<Appointment>,IAppointmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentRepository(ApplicationDbContext dbContext):base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Appointment>> GetByRoomIdOrDoctorId(Guid? roomOrDeviceId,string? doctorUserId)
    {
        return _dbContext.Appointments
            .Where(a => (a.RoomOrDeviceId == roomOrDeviceId || a.ApplicationUserId == doctorUserId) && a.IsDeleted == false)
            .Include(a => a.RoomOrDevice)
            .Include(a => a.ApplicationUser)
            .Include(a => a.MedicalService)
            .Include(a => a.Disease);
    }

    public async Task<Appointment> Create(AppointmentDto newObject)
    {
        var appointment = new Appointment
        {
            Start = newObject.Start,
            End = newObject.End,
            RoomOrDeviceId = newObject.RoomOrDeviceId,
            ApplicationUserId = newObject.ApplicationUserId,
            MedicalServiceId = newObject.MedicalServiceId,
            Phone = newObject.Phone,
            DiseaseId = newObject.DiseaseId,
        };

        var dbAppointment = await _dbContext.Appointments.AddAsync(appointment);
        await _dbContext.SaveChangesAsync();

        return dbAppointment.Entity;
    }
    
    public async Task<Appointment> Delete(Guid id)
    {
        var entry = await _dbContext.Appointments.FirstOrDefaultAsync(t => t.Id == id);
        if (entry is null)
        {
            return null;
        }

        entry.IsDeleted = true;
        await Save();
        return entry;
    }
}