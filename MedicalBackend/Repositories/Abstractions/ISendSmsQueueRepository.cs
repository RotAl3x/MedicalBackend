using MedicalBackend.Entities;
using Twilio.Rest.Api.V2010.Account;

namespace MedicalBackend.Repositories.Abstractions;

public interface ISendSmsQueueRepository
{
    Task Create(Guid appointmentId, string message, DateTime sendAfterDate);
    Task<List<SendSmsQueue>> GetSmsWhoNeedToBeSend();
    Task UpdateStatusSent(Guid sendSmsId, string sid, MessageResource.StatusEnum status);
    Task UpdateStatusFromTwilio(string sid, MessageResource.StatusEnum status);
    Task Delete(Guid sendSmsId);
    Task DeleteByAppointmentId(Guid appointmentId);
}