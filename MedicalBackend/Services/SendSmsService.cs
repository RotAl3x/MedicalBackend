using Coravel.Invocable;
using MedicalBackend.Repositories.Abstractions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MedicalBackend.Services;

public class SendSmsService : IInvocable
{
    private readonly IConfiguration _configuration;
    private readonly ISendSmsQueueRepository _sendSmsQueueRepository;

    public SendSmsService(IConfiguration configuration, ISendSmsQueueRepository sendSmsQueueRepository)
    {
        _configuration = configuration;
        _sendSmsQueueRepository = sendSmsQueueRepository;
    }

    public async Task Invoke()
    {
        string accountSid = _configuration.GetSection("Twilio:accountSid").Value;
        string authToken = _configuration.GetSection("Twilio:authToken").Value;

        TwilioClient.Init(accountSid, authToken);

        var smsWhoNeedToBeSend = await _sendSmsQueueRepository.GetSmsWhoNeedToBeSend();
        await Parallel.ForEachAsync(smsWhoNeedToBeSend, async (sms, cancellationToken) =>
        {
            if (sms.Sid != null)
            {
                var status = await GetStatus(sms.Sid);
                await _sendSmsQueueRepository.UpdateStatusFromTwilio(sms.Sid, status);
            }
        });
        
        smsWhoNeedToBeSend = await _sendSmsQueueRepository.GetSmsWhoNeedToBeSend();

        await Parallel.ForEachAsync(smsWhoNeedToBeSend, async (sms, cancellationToken) =>
        {
            var response = await SendSms(sms.Appointment.Phone, sms.Message);
            await _sendSmsQueueRepository.UpdateStatusSent(sms.Id, response.Sid, response.Status);
        });
    }

    private async Task<MessageResource> SendSms(string number, string message)
    {
        var response = MessageResource.CreateAsync(
            body: message,
            from: new Twilio.Types.PhoneNumber(_configuration.GetSection("Twilio:number").Value),
            to: new Twilio.Types.PhoneNumber(number)
        );
        return await response;
    }

    private async Task<MessageResource.StatusEnum> GetStatus(string sid)
    {
        var response = await MessageResource.FetchAsync(pathSid: sid);
        return response.Status;
    }
}