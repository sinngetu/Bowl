using Bowl.Common;
using Bowl.Data;
using Bowl.Models.Entities;
using Bowl.Models.Response;

namespace Bowl.Services.Business
{
    public interface IHotlistService
    {
        (ErrorType, HotlistResponse) GetHotlistByHash(string hash);
        (ErrorType, List<HotlistResponse>) GetHotlistByContent(string content);
        (ErrorType, List<HotlistResponse>) GetHotlistByDate(DateTime start, DateTime end);
        (ErrorType, List<HotlistResponse>) GetHotlistByPlatform(int platform);
        List<HotlistResponse> GetWeiboList();
        ErrorType SetWeiboList(List<string> hashes);
    }

    public class HotlistService : IHotlistService
    {
        private static List<HotlistResponse> weiboList = new List<HotlistResponse>();

        private readonly ILogger<HotlistService> _logger;
        private readonly ApplicationDbContext _context;

        public HotlistService(ILogger<HotlistService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        private HotlistResponse HotlistToResponse(Hotlist data)
        {
            return new HotlistResponse
            {
                Hash = data.Hash,
                Content = data.Content,
                Platform = data.Platform,
                Date = data.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                Link = data.Link,
            };
        }

        public (ErrorType, HotlistResponse) GetHotlistByHash(string hash)
        {
            try
            {
                var result = _context.Hotlist.Single(r => r.Hash == hash);
                var errorType = result == null ? ErrorType.NotExist : ErrorType.NoError;

                return (errorType, HotlistToResponse(result!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{hash}", hash);
                return (ErrorType.DatabaseError, null);
            }
        }

        public (ErrorType, List<HotlistResponse>) GetHotlistByContent(string content)
        {
            try
            {
                var data = _context.Hotlist
                    .Where(r => r.Content.Contains(content))
                    .Select(HotlistToResponse)
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{content}", content);
                return (ErrorType.DatabaseError, new List<HotlistResponse>());
            }
        }

        public (ErrorType, List<HotlistResponse>) GetHotlistByDate(DateTime start, DateTime end)
        {
            try
            {
                if (start > end)
                    return (ErrorType.InvalidArgument, new List<HotlistResponse>());

                var data = _context.Hotlist
                    .Where(r => r.Date > start && r.Date < end)
                    .OrderByDescending(r => r.Date)
                    .Select(HotlistToResponse)
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{start}, {end}", start, end);
                return (ErrorType.DatabaseError, new List<HotlistResponse>());
            }
        }

        public (ErrorType, List<HotlistResponse>) GetHotlistByPlatform(int platform)
        {
            try
            {
                var data = _context.Hotlist
                    .Where(r => r.Platform == platform)
                    .Select(HotlistToResponse)
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{platform}", platform);
                return (ErrorType.DatabaseError, new List<HotlistResponse>());
            }
        }

        public List<HotlistResponse> GetWeiboList()
        {
            return weiboList;
        }

        public ErrorType SetWeiboList(List<string> hashes)
        {
            try
            {
                var rawList = _context.Hotlist
                    .Where(r => hashes.Contains(r.Hash))
                    .ToList();

                var _default = new Hotlist();

                var list = hashes
                    .Select(hash => rawList.FirstOrDefault(item => item.Hash == hash) ?? _default)
                    .ToList();

                if (list.Contains(_default))
                    _logger.LogWarning(Utils.GetClassNameAndMethodName() + "{hashes}", hashes);

                weiboList = list
                    .Select(HotlistToResponse)
                    .ToList();

                return ErrorType.NoError;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{hashes}", hashes);

                return ErrorType.UnknowError;
            }
        }
    }
}
