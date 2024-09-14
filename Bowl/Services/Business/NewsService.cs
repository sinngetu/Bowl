using Bowl.Common;
using Bowl.Data;
using Bowl.Models.Entities;
using Bowl.Models.Request;
using Bowl.Models.Response;

namespace Bowl.Services.Business
{
    public interface INewsService
    {
        (ErrorType, bool) AddNews(List<News> news);
        (ErrorType, bool) UpdateNews(string hash, News news);
        (ErrorType, NewsResponse) GetNewsByHash(string hash);
        (ErrorType, List<NewsResponse>) GetNews(GetNewsParameters parameters);
        (ErrorType, bool) AddBossNews(List<Boss> news);
        (ErrorType, List<BossNewsResponse>) GetBossNewsByDate(DateTime start, DateTime end);
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

        private NewsResponse NewsToResponse(News news)
        {
            return new NewsResponse
            {
                Hash = news.Hash,
                Link = news.Link,
                Medium = news.Medium,
                Title = news.Title,
                Date = news.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = news.Status,
                Keyword = news.Keyword,
                Tags = string.IsNullOrEmpty(news.Tags)
                    ? []
                    : news.Tags
                        .Split(",")
                        .Select(str => int.Parse(str))
                        .ToArray()
            };
        }

        private BossNewsResponse BossNewsToResponse(Boss news)
        {
            return new BossNewsResponse
            {
                Hash = news.Hash,
                Link = news.Link,
                Content = news.Content,
                Date = news.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                Type = news.Type,
            };
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
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{news}", news);
                return (ErrorType.DatabaseError, false);
            }
        }

        public (ErrorType, bool) UpdateNews(string hash, News news)
        {
            try
            {
                var record = _context.News.Single(r => r.Hash == hash);

                if (record == null)
                    return (ErrorType.NotExist, false);

                record.Tags = news.Tags ?? record.Tags;
                _context.News.Update(record);
                _context.SaveChanges();

                return (ErrorType.NoError, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{hash}, {news}", hash, news);
                return (ErrorType.DatabaseError, false);
            }
        }

        public (ErrorType, NewsResponse) GetNewsByHash(string hash)
        {
            try
            {
                var record = _context.News.Single(r => r.Hash == hash);

                if (record == null)
                    return (ErrorType.NotExist, null);

                return (ErrorType.NoError, NewsToResponse(record));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{hash}", hash);
                return (ErrorType.DatabaseError, null);
            }
        }

        public (ErrorType, List<NewsResponse>) GetNews(GetNewsParameters parameters)
        {
            try
            {
                var query = _context.News.AsQueryable()
                    .Where(r => r.Date > parameters.Start && r.Date < parameters.End)
                    .Where(r => r.Status == parameters.Status);

                if (parameters.Medium.Length != 0)
                    query = query.Where(r => parameters.Medium.Contains(r.Medium));
                if (!string.IsNullOrEmpty(parameters.Title))
                    query = query.Where(r => r.Title.Contains(parameters.Title));

                var data = query.ToList();

                if (parameters.Tags.Length != 0)
                {
                    var tags = data
                        .SelectMany(r => (r.Tags ?? "")
                            .Split(",")
                            .Where(r => !string.IsNullOrEmpty(r))
                            .Select(r => int.Parse(r))
                        )
                        .Distinct()
                        .ToList();

                    data = data
                        .Where(r => parameters.Tags.Any(tag => tags.Contains(tag)))
                        .ToList();
                }

                var result = data
                    .OrderByDescending(r => r.Date)
                    .Select(NewsToResponse)
                    .ToList();

                return (ErrorType.NoError, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{parameters}", parameters);
                return (ErrorType.DatabaseError, new List<NewsResponse>());
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
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{news}", news);
                return (ErrorType.DatabaseError, false);
            }
        }

        public (ErrorType, List<BossNewsResponse>) GetBossNewsByDate(DateTime start, DateTime end)
        {
            start = start.ToLocalTime();
            end = end.ToLocalTime();

            try
            {
                var data = _context.Boss
                    .Where(r => r.Date >= start && r.Date <= end)
                    .Select(BossNewsToResponse)
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{start}, {end}", start, end);
                return (ErrorType.DatabaseError, new List<BossNewsResponse>());
            }
        }
    }
}
