using MedicalBackend.Entities;

namespace MedicalBackend.Hub.Abstractions;

public interface IMessageHubClient
{
    Task SendAppointmentsToUser(List<Appointment> appointments);
    Task SendAppointmentToUser(Appointment appointment);
}