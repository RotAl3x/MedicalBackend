using MedicalBackend.Database;
using MedicalBackend.Entities;
using MedicalBackend.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MedicalBackend.Repositories;

public class SendSmsQueueRepository : ISendSmsQueueRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SendSmsQueueRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Create(Guid appointmentId, string message, DateTime sendAfterDate)
    {
        var sendSms = new SendSmsQueue()
        {
            AppointmentId = appointmentId,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            SendAfterDate = sendAfterDate,
            Message = message,
            RetryCount = 0,
            Sent = false
        };
        await _dbContext.SendSmsQueue.AddAsync(sendSms);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<SendSmsQueue>> GetSmsWhoNeedToBeSend()
    {
        var smsList = await _dbContext.SendSmsQueue.Include(s => s.Appointment).Where(s =>
            s.SendAfterDate <= DateTime.UtcNow &&
            s.RetryCount < 5 &&
            !s.Sent &&
            !s.IsDeleted &&
            !s.Appointment.IsDeleted
        ).ToListAsync();

        foreach (var sms in smsList)
        {
            sms.Updated = DateTime.UtcNow;
            sms.RetryCount = sms.RetryCount++;
        }

        await _dbContext.SaveChangesAsync();

        return smsList;
    }

    public async Task UpdateStatusSent(Guid sendSmsId)
    {
        var sms = await _dbContext.SendSmsQueue.FirstOrDefaultAsync(s => s.Id == sendSmsId);
        if (sms is null)
        {
            return;
        }
        sms.Sent = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid sendSmsId)
    {
        var sms = await _dbContext.SendSmsQueue.FirstOrDefaultAsync(s => s.Id == sendSmsId);
        if (sms is null)
        {
            return;
        }
        sms.IsDeleted = true;

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteByAppointmentId(Guid appointmentId)
    {
        var sms = await _dbContext.SendSmsQueue.Where(s => s.AppointmentId == appointmentId).ToListAsync();
        if (!sms.Any())
        {
            return;
        }

        foreach (var s in sms)
        {
            s.IsDeleted = true;
        }

        await _dbContext.SaveChangesAsync();
    }
}