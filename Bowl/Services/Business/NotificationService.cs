using Bowl.Common;
using Bowl.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Bowl.Services.Business
{
    public interface INotificationService
    {
        (ErrorType, bool) Notify(int port, string data);
    }

    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly ApplicationDbContext _context;

        public NotificationService(ILogger<NotificationService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public (ErrorType, bool) Notify(int port, string data)
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Loopback, port);
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(remote);

                byte[] msg = Encoding.UTF8.GetBytes(data);
                sender.Send(msg);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return (ErrorType.NoError, true);
            }
            catch (SocketException ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{port}, {data}", port, data);
                return (ErrorType.NetError, false);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{port}, {data}", port, data);
                return (ErrorType.InvalidArgument, false);
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{port}, {data}", port, data);
                return (ErrorType.SocketError, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{port}, {data}", port, data);
                return (ErrorType.UnknowError, false);
            }
        }
    }
}
