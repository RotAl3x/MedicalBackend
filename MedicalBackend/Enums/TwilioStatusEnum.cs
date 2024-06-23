namespace MedicalBackend.Enums;

public enum TwilioStatusEnum
{
    queued = 1,
    sending,
    sent,
    failed,
    delivered,
    undelivered,
    received,
    accepted,
    scheduled,
    read,
    partiallyDelivered,
    canceled
}