using Bowl.Common;
using Bowl.Data;
using Bowl.Models.Entities;

namespace Bowl.Services.Business
{
    public interface IPlatformService
    {
        (ErrorType, List<Platform>) GetAllPlatform();
    }

    public class PlatformService : IPlatformService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PlatformService> _logger;

        public PlatformService(ApplicationDbContext context, ILogger<PlatformService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public (ErrorType, List<Platform>) GetAllPlatform()
        {
            try
            {
                return (ErrorType.NoError, _context.Platform.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName());

                return (ErrorType.DatabaseError, new List<Platform>());
            }
        }
    }
}
