using MedicalBackend.DTOs;
using MedicalBackend.Entities;

namespace MedicalBackend.Repositories.Abstractions;

public interface IAppointmentRepository:IBaseRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByRoomIdOrDoctorId(Guid? roomOrDeviceId, string? doctorUserId);
    Task<Appointment> Create(AppointmentDto newObject);
    Task<Appointment> Delete(Guid id);
    Task<IList<Appointment>> GetCabinetFreeDays();
    Task<IList<Appointment>> GetDoctorFreeDays(string doctorId);
}