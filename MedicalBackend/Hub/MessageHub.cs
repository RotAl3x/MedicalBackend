using MedicalBackend.Entities;
using MedicalBackend.Hub.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace MedicalBackend.Hub;

public class MessageHub : Hub<IMessageHubClient>
{
    public async Task SendAppointmentsToUser(List<Appointment> appointments)
    {
        await Clients.All.SendAppointmentsToUser(appointments);
    }

    public async Task SendAppointmentToUser(Appointment appointment)
    {
        await Clients.All.SendAppointmentToUser(appointment);
    }
}