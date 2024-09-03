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
        (ErrorType, ResponseNews) GetNewsByHash(string hash);
        (ErrorType, List<ResponseNews>) GetNews(GetNewsParameters parameters);
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

        private ResponseNews NewsToResponseNews(News news)
        {
            return new ResponseNews
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
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{hash}, {news}", hash, news);
                return (ErrorType.DatabaseError, false);
            }
        }

        public (ErrorType, ResponseNews) GetNewsByHash(string hash)
        {
            try
            {
                var record = _context.News.SingleOrDefault(r => r.Hash == hash);

                if (record == null)
                    return (ErrorType.NotExist, null);

                return (ErrorType.NoError, NewsToResponseNews(record));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{hash}", hash);
                return (ErrorType.DatabaseError, null);
            }
        }

        public (ErrorType, List<ResponseNews>) GetNews(GetNewsParameters parameters)
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
                    .Select(NewsToResponseNews)
                    .ToList();

                return (ErrorType.NoError, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{parameters}", parameters);
                return (ErrorType.DatabaseError, new List<ResponseNews>());
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
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{hashes}", hashes);
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
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{content}", content);
                return (ErrorType.DatabaseError, new List<Boss>());
            }
        }

        public (ErrorType, List<Boss>) GetBossNewsByDate(DateTime start, DateTime end)
        {
            start = start.ToLocalTime();
            end = end.ToLocalTime();

            try
            {
                var data = _context.Boss
                    .Where(r => r.Date >= start && r.Date <= end)
                    .ToList();

                return (ErrorType.NoError, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{start}, {end}", start, end);
                return (ErrorType.DatabaseError, new List<Boss>());
            }
        }


    }
}
