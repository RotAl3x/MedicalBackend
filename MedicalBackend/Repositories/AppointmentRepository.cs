using MedicalBackend.Database;
using MedicalBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalBackend.Repositories;

public class AppointmentRepository: BaseRepository<Appointment>
{
    private readonly ApplicationDbContext _dbContext;

    public AppointmentRepository(ApplicationDbContext dbContext):base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Appointment>> GetByRoomIdOrDoctorId(Guid? roomOrDeviceId,string? doctorUserId)
    {
        return _dbContext.Appointments
            .Where(a => a.RoomOrDeviceId == roomOrDeviceId || a.ApplicationUserId == doctorUserId)
            .Include(a => a.RoomOrDevice)
            .Include(a => a.ApplicationUser)
            .Include(a => a.MedicalService)
            .Include(a => a.Disease);

    }
}