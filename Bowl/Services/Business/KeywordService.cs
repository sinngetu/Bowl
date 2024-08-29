using Bowl.Common;
using Bowl.Data;
using Bowl.Models.Entities;

namespace Bowl.Services.Business
{
    public interface IKeywordService
    {
        (ErrorType, List<Keyword>) GetAllKeyword();
        (ErrorType, bool) AddKeyword(Keyword keyword);
        (ErrorType, bool) RemoveKeyword(int id);
        (ErrorType, bool) UpdateKeyword(int id, Keyword keyword);
    }

    public class KeywordService : IKeywordService
    {
        private readonly ILogger<KeywordService> _logger;
        private readonly ApplicationDbContext _context;

        public KeywordService(ILogger<KeywordService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public (ErrorType, List<Keyword>) GetAllKeyword()
        {
            try
            {
                return (ErrorType.NoError, _context.Keyword.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName());
                return (ErrorType.DatabaseError, new List<Keyword>());
            }
        }

        public (ErrorType, bool) AddKeyword(Keyword keyword)
        {
            try
            {
                _context.Keyword.Add(keyword);
                _context.SaveChanges();

                return (ErrorType.NoError, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{keyword}", keyword);

                return (ErrorType.DatabaseError, false);
            }
        }

        public (ErrorType, bool) RemoveKeyword(int id)
        {
            try
            {
                var record = _context.Keyword.SingleOrDefault(r => r.Id == id);
                if (record != null)
                {
                    _context.Keyword.Remove(record);
                    _context.SaveChanges();
                }

                return (ErrorType.NoError, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{id}", id);

                return (ErrorType.DatabaseError, false);
            }
        }

        public (ErrorType, bool) UpdateKeyword(int id, Keyword keyword)
        {
            try
            {
                var record = _context.Keyword.SingleOrDefault(r => r.Id == id);

                if (record == null)
                    return (ErrorType.NotExist, false);

                record.Word = keyword.Word ?? record.Word;
                record.Extend = keyword.Extend ?? record.Extend;
                _context.Keyword.Update(record);
                _context.SaveChanges();

                return (ErrorType.NoError, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Utils.GetClassNameAndMethodName() + "{id}, {keyword}", id, keyword);

                return (ErrorType.DatabaseError, false);
            }
        }
    }
}
