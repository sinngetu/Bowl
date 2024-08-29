using Bowl.Common;
using Bowl.Data;
using Bowl.Models.Entities;

namespace Bowl.Services.Business
{
    public interface IMediaService
    {
        (ErrorType, List<Media>) GetAllMedia();
    }

    public class MediaService : IMediaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MediaService> _logger;

        public MediaService(ApplicationDbContext context, ILogger<MediaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public (ErrorType, List<Media>) GetAllMedia()
        {
            try
            {
                return (ErrorType.NoError, _context.Media.ToList());
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, ex);
                return (ErrorType.DatabaseError, new List<Media>());
            }
        }
    }
}
