namespace MedicalBackend.Services;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly AppointmentSocketService _appointmentSocketService;

    public Worker(ILogger<Worker> logger,AppointmentSocketService appointmentSocketService)
    {
        _logger = logger;
        _appointmentSocketService = appointmentSocketService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker start: {time}",DateTimeOffset.Now);
        
        _appointmentSocketService.Start();
    }
}