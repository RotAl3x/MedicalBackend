using Fleck;

namespace MedicalBackend.Services;

public class AppointmentSocketService
{
    private readonly ILogger<AppointmentSocketService> _logger;
    private readonly List<IWebSocketConnection> _socket = new();
    private readonly WebSocketServer _server = new ("ws://0.0.0.0:8181");

    public AppointmentSocketService(ILogger<AppointmentSocketService> logger)
    {
        _logger = logger;
    }

    public void Start()
    {
        _server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                _logger.LogInformation("Connection open.");
                _socket.Add(socket);
            };
            socket.OnClose = () =>
            {
                _logger.LogInformation("Connection closed.");
                _socket.Remove(socket);
            };
            socket.OnMessage = message =>
            {
                _logger.LogInformation("Says:" + message);
                _socket.ForEach(s => s.Send("Teset" + message));
            };
        });
    }
    
    
}