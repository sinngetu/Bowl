using Bowl.Common;
using Bowl.Data;
using Bowl.Models.Entities;

namespace Bowl.Services.Business
{
    public interface IHotlistService
    {
        (ErrorType, Hotlist) GetHotlistByHash(string hash);
        (ErrorType, List<Hotlist>) GetHotlistByContent(string content);
        (ErrorType, List<Hotlist>) GetHotlistByDate(DateTime start, DateTime end);
        (ErrorType, List<Hotlist>) GetHotlistByPlatform(int platform);
    }

    public class HotlistService : IHotlistService
    {
        private static List<Hotlist> weiboList = new List<Hotlist>();

        private readonly ILogger<HotlistService> _logger;
        private readonly ApplicationDbContext _context;

        public HotlistService(ILogger<HotlistService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public (ErrorType, Hotlist) GetHotlistByHash(string hash)
        {
            try
            {
                var result = _context.Hotlist.FirstOrDefault(r => r.Hash == hash);
                var errorType = result == null ? ErrorType.NotExist : ErrorType.NoError;

                return (errorType, result);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, hash, ex);
                return (ErrorType.DatabaseError, null);
            }
        }

        public (ErrorType, List<Hotlist>) GetHotlistByContent(string content)
        {
            try
            {
                var data = _context.Hotlist
                    .Where(r => r.Content.Contains(content))
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, content, ex);
                return (ErrorType.DatabaseError, new List<Hotlist>());
            }
        }

        public (ErrorType, List<Hotlist>) GetHotlistByDate(DateTime start, DateTime end)
        {
            try
            {
                if (start > end)
                    return (ErrorType.InvalidArgument, new List<Hotlist>());

                var data = _context.Hotlist
                    .Where(r => r.Date > start && r.Date < end)
                    .OrderByDescending(r => r.Date)
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, start, end, ex);
                return (ErrorType.DatabaseError, new List<Hotlist>());
            }
        }

        public (ErrorType, List<Hotlist>) GetHotlistByPlatform(int platform)
        {
            try
            {
                var data = _context.Hotlist
                    .Where(r => r.Platform == platform)
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, platform, ex);
                return (ErrorType.DatabaseError, new List<Hotlist>());
            }
        }

        public List<Hotlist> GetWeiboList()
        {
            return weiboList;
        }

        public void SetWeiboList(List<string> hashes)
        {
            var rawList = _context.Hotlist
                .Where(r => hashes.Contains(r.Hash))
                .ToList();

            var _default = new Hotlist();

            var list = hashes
                .Select(hash => rawList.FirstOrDefault(item => item.Hash == hash) ?? _default)
                .ToList();

            if (list.Contains(_default))
                Utils.Log(_logger.LogWarning, hashes);

            weiboList = list;
        }
    }
}
