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
        foreach (var sms in smsWhoNeedToBeSend)
        {
            var response = SendSms(sms.Appointment.Phone, sms.Message);
            if (response.Status == MessageResource.StatusEnum.Queued)
            {
               await _sendSmsQueueRepository.UpdateStatusSent(sms.Id);
            }
        };
    }

    private MessageResource SendSms(string number, string message)
    {
        var response = MessageResource.Create(
            body: message,
            from: new Twilio.Types.PhoneNumber(_configuration.GetSection("Twilio:number").Value),
            to: new Twilio.Types.PhoneNumber(number)
        );
        return response;
    }
}