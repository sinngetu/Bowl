using Bowl.Common;
using Bowl.Data;
using Bowl.Models.Entities;
using Bowl.Models.Request.News;

namespace Bowl.Services.Business
{
    public interface INewsService
    {
        (ErrorType, bool) AddNews(List<News> news);
        (ErrorType, bool) UpdateNews(string hash, News news);
        (ErrorType, News) GetNewsByHash(string hash);
        (ErrorType, List<News>) GetNews(GetNewsParameters parameters);
        (ErrorType, bool) AddBossNews(List<Boss> news);
        (ErrorType, List<Boss>) GetBossNewsByHashes(List<string> hashes);
        (ErrorType, List<Boss>) GetBossNewsByContent(string content);
        (ErrorType, List<Boss>) GetBossNewsByDate(DateTime start, DateTime end);
    }

    public class NewsService : INewsService
    {
        private readonly ILogger<NewsService> _logger;
        private readonly ApplicationDbContext _context;

        public NewsService(ILogger<NewsService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public (ErrorType, bool) AddNews(List<News> news)
        {
            try
            {
                var hashes = news.Select(r => r.Hash).ToList();
                var hasRepeat = _context.News
                    .Where(r => hashes.Contains(r.Hash))
                    .ToArray()
                    .Length > 0;

                if (hasRepeat)
                    return (ErrorType.RecordDuplication, false);

                _context.News.AddRange(news);
                _context.SaveChanges();
                return (ErrorType.NoError, true);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, news, ex);
                return (ErrorType.DatabaseError, false);
            }
        }

        public (ErrorType, bool) UpdateNews(string hash, News news)
        {
            try
            {
                var record = _context.News.SingleOrDefault(r => r.Hash == hash);

                if (record == null)
                    return (ErrorType.NotExist, false);

                record.Tags = news.Tags ?? record.Tags;
                _context.News.Update(record);
                _context.SaveChanges();

                return (ErrorType.NoError, true);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, hash, news, ex);
                return (ErrorType.DatabaseError, false);
            }
        }

        public (ErrorType, News) GetNewsByHash(string hash)
        {
            try
            {
                var record = _context.News.SingleOrDefault(r => r.Hash == hash);

                if (record == null)
                    return (ErrorType.NotExist, null);

                return (ErrorType.NoError, record);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, hash, ex);
                return (ErrorType.DatabaseError, null);
            }
        }

        private bool NewsFilter(News news, GetNewsParameters parameters)
        {
            var tags = (news.Tags ?? "")
                .Split(",")
                .Where(r => r != "")
                .Select(r => int.Parse(r))
                .ToArray();

            var isInMedium = parameters.Medium.Length != 0 ? parameters.Medium.Contains(news.Medium) : true;
            var containsTitle = news.Title.Contains(parameters.Title);
            var isStatus = news.Status == parameters.Status;
            var isInTags = parameters.Tags.Length != 0 ? parameters.Tags.Aggregate(false, (result, tag) => result || tags.Contains(tag)) : true;
            var isInDateRange = parameters.Start > news.Date && parameters.End < news.Date;

            return isInMedium && containsTitle && isStatus && isInTags && isInDateRange;
        }

        public (ErrorType, List<News>) GetNews(GetNewsParameters parameters)
        {
            try
            {
                var data = _context.News
                    .Where(r => NewsFilter(r, parameters))
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, parameters, ex);
                return (ErrorType.DatabaseError, new List<News>());
            }
        }

        public (ErrorType, bool) AddBossNews(List<Boss> news)
        {
            try
            {
                var hashes = news.Select(r => r.Hash).ToList();
                var hasRepeat = _context.Boss
                    .Where(r => hashes.Contains(r.Hash))
                    .ToArray()
                    .Length > 0;

                if (hasRepeat)
                    return (ErrorType.RecordDuplication, false);

                _context.Boss.AddRange(news);
                _context.SaveChanges();

                return (ErrorType.NoError, true);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, news, ex);
                return (ErrorType.DatabaseError, false);
            }
        }

        public (ErrorType, List<Boss>) GetBossNewsByHashes(List<string> hashes)
        {
            try
            {
                var data = _context.Boss
                    .Where(r => hashes.Contains(r.Hash))
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, hashes, ex);
                return (ErrorType.DatabaseError, new List<Boss>());
            }
        }

        public (ErrorType, List<Boss>) GetBossNewsByContent(string content)
        {
            try
            {
                var data = _context.Boss
                    .Where(r => r.Content.Contains(content))
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, content, ex);
                return (ErrorType.DatabaseError, new List<Boss>());
            }
        }

        public (ErrorType, List<Boss>) GetBossNewsByDate(DateTime start, DateTime end)
        {
            try
            {
                var data = _context.Boss
                    .Where(r => r.Date >= start && r.Date <= end)
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                Utils.Log(_logger.LogError, start, end, ex);
                return (ErrorType.DatabaseError, new List<Boss>());
            }
        }


    }
}
