using MedicalBackend.Entities;

namespace MedicalBackend.Repositories.Abstractions;

public interface ISendSmsQueueRepository
{
    Task Create(Guid appointmentId, string message, DateTime sendAfterDate);
    Task<List<SendSmsQueue>> GetSmsWhoNeedToBeSend();
    Task UpdateStatusSent(Guid sendSmsId);
    Task Delete(Guid sendSmsId);
    Task DeleteByAppointmentId(Guid appointmentId);
}